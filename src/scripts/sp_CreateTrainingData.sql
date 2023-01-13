/****** Object:  StoredProcedure [dbo].[sp_Create_TrainingData]    Script Date: 04.12.2020 10:13:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[sp_Create_TrainingData] as 
BEGIN
-- Metrics:
--- SQL Tier Basic 5 DTU 4 EUR / month
---- 2020-11-01 ca 730k lines -> >60 min
---- 2020-11-02 744k lines -> 36 min
--- SQL Tier Standard 10 DTU 12 EUR / month
---- 2020-11-02 742k lines -> 40 min (!!)
---- 2020-11-02 743k lines -> 9,5 min, truncate enabled



IF ISNULL(OBJECT_ID(N'kettler_3a_fact'),0) <> 0 DROP TABLE kettler_3a_fact
IF ISNULL(OBJECT_ID(N'kettler_3b_agg'),0) <> 0 DROP TABLE kettler_3b_agg


IF ISNULL(OBJECT_ID(N'kettler_1'),0) = 0 
CREATE TABLE [kettler_1] (
[Id] bigint IDENTITY(1,1) NOT NULL,
[Xml_Column] nvarchar(255),
[SSIS_StartTime] nvarchar(255),
[SSIS_FileName] nvarchar(255),
)


INSERT INTO [kettler_1] ([Xml_Column],[SSIS_StartTime],[SSIS_FileName])
SELECT 
	[XmlString],
	[CreatedOn],
	[FileName] 
FROM [Kettler_0] WHERE [FileName] NOT IN 
	(SELECT DISTINCT SSIS_Filename FROM [kettler_1])

-- drop table #t1_pulse
SELECT
		REPLACE(REPLACE(REPLACE(REPLACE([Xml_Column],'Pulse',''),'<',''),'>',''),'/','') AS Value,
		RIGHT([SSIS_FileName],24) AS [SSIS_FileName],
		ROW_NUMBER() OVER (PARTITION BY [SSIS_FileName] ORDER BY [Id]) AS Interval
	into #t1_pulse
	FROM [kettler_1]
	WHERE [Xml_Column] LIKE '%Pulse%'

-- drop table #t1_pow
SELECT
		REPLACE(REPLACE(REPLACE(REPLACE([Xml_Column],'Power',''),'<',''),'>',''),'/','') AS Value,
		RIGHT([SSIS_FileName],24) AS [SSIS_FileName],
		ROW_NUMBER() OVER (PARTITION BY [SSIS_FileName] ORDER BY [Id]) AS Interval
	into #t1_pow
	FROM [kettler_1]
	WHERE [Xml_Column] LIKE '%Power%'

-- drop table #t1_rpm
SELECT
		REPLACE(REPLACE(REPLACE(REPLACE([Xml_Column],'RPM',''),'<',''),'>',''),'/','') AS Value,
		RIGHT([SSIS_FileName],24) AS [SSIS_FileName],
		ROW_NUMBER() OVER (PARTITION BY [SSIS_FileName] ORDER BY [Id]) AS Interval
	into #t1_rpm
	FROM [kettler_1]
	WHERE [Xml_Column] LIKE '%RPM%' 

-- drop table #t1; 25sec / 1800 rows 
SELECT 
	[pulse].[Interval],
	CAST(REPLACE([pulse].[Value],CHAR(9),'') AS INT) AS PulseValue,
	CAST(REPLACE([pow].[Value],CHAR(9),'') AS INT) AS PowerValue,
	CAST(REPLACE([rpm].[Value],CHAR(9),'') AS INT) AS RPMValue,
	[pulse].[SSIS_FileName],
	TRY_PARSE(dbo.fx_dottedDateToDashed(LEFT([pulse].[SSIS_FileName],10)) AS DATE ) AS [Date],
	ROUND(CAST([pulse].[Interval] AS FLOAT)*10/60,1) AS TimePassed
INTO #t1
FROM #t1_pulse pulse
JOIN #t1_pow pow ON [pulse].[SSIS_FileName]=pow.[SSIS_FileName] AND [pulse].[Interval]=[pow].[Interval]
JOIN #t1_rpm rpm ON [pulse].[SSIS_FileName]=rpm.[SSIS_FileName] AND [pulse].[Interval]=rpm.[Interval]

-- drop table #t2
SELECT		[SSIS_FileName]
			,[Date]
			,MAX([TimePassed]) AS Duration_in_min 
			,AVG([PulseValue]) AS Avg_Pulse
			,AVG([PowerValue]) AS Avg_Power
			,AVG([RPMValue]) AS Avg_RPM
INTO		#t2
FROM		#t1
GROUP BY	[SSIS_FileName], [Date] 

SELECT	* INTO	kettler_3a_fact	FROM [#t1]
SELECT	* INTO	kettler_3b_agg	FROM [#t2]

-- error correction: delete duplicates
delete from kettler_3a_fact where [SSIS_FileName] in ('04.02.2017 13h58m04s.xml','18.09.2020 17h55m32s.xml', '15.02.2018 20h53m02s.xml')
delete from kettler_3b_agg where [SSIS_FileName] in ('04.02.2017 13h58m04s.xml','18.09.2020 17h55m32s.xml', '15.02.2018 20h53m02s.xml')



--2020-10-29 add aux columns
-- get MaxInterval
select	*
--		,round(cast(Interval as float) / cast(Maxinterval as float) ,5) 
		,cast(cast(Interval as float) / cast(Maxinterval as float) / cast(0.05 as float) as int) * 0.05 as TimePassed_pct 
into #t4
from 
(	SELECT [Interval]
		  ,a.[SSIS_FileName]
		  ,CAST(PowerValue AS FLOAT)/130 * CAST(RPMValue AS FLOAT)/50 / 180 as Score_10sec
		  ,(select max(Interval) from [dbo].[kettler_3a_fact] sub where sub.SSIS_FileName = a.SSIS_FileName) as MaxInterval
	  FROM [dbo].[kettler_3a_fact] a
) tab

alter table dbo.kettler_3a_fact
add 
	[Score_10sec] float,
	[TimePassed_Pct] float

alter table dbo.kettler_3b_agg
add 
	[HourStart] int,
	[Daytime] varchar(50),
	[Streak] int

update dbo.kettler_3a_fact set [Score_10sec] = CAST(PowerValue AS FLOAT)/130 * CAST(RPMValue AS FLOAT)/50 / 180 
update a set TimePassed_Pct = b.TimePassed_pct
	from dbo.kettler_3a_fact a join #t4 b ON a.SSIS_FileName = b.SSIS_FileName and a.Interval = b.Interval

update dbo.kettler_3b_agg set [HourStart] = cast(left(right([SSIS_FileName], 13),2) as int)
update dbo.kettler_3b_agg set [Daytime] = case 
	when [HourStart] between 0 and 11 then 'morning' 
	when [HourStart] between 12 and 18 then 'day' 
	when [HourStart] between 19 and 23 then 'night' 
	end

/*Find Streaks*/
-- drop table #streak
SELECT	Rank() OVER (Order by d.Date_Id) as Id
		,cast(d.Date_Id as date) as Date_Id, a.Date
  into #streak
  FROM	[dbo].kettler_date_dim d 
  LEFT JOIN [dbo].[kettler_3b_agg] a on d.Date_Id = a.Date

-- drop table #streak2
  select *
  , RANK() over (order by Date_Id) as xDE 
  into #streak2
  from #streak
  where Date is null
-- drop table #streak3
  select	l.Id,
			l.Date_Id,
			l.Id - r.Id -1 as Streak 
  into #streak3
  from #streak2 l JOIN #streak2 r on l.xDE = r.xDE + 1

  update a set Streak = t.Streak
  from dbo.kettler_3b_agg a JOIN #streak3 t ON a.Date = dateadd(d, -1, t.Date_Id)
 
 


truncate table dbo.Kettler_0



-- Score_10sec=([PowerValue]/130) * ([RPMValue]/50) / 180
-- TimePassed==ROUND([TimePassed]/RELATED(fact_TrainingData_2[Duration_in_min]);2)


-- drop table #t1
-- drop table #t2
-- drop table #t3

-- drop table #t3
--SELECT	t1.*
--		,t2.[Avg_Power]
--		,t3.[Avg_RPM]
--		,TRY_PARSE(meta.fx_dottedDateToDashed(LEFT([t1].[SSIS_FileName],10)) AS DATE ) AS [Date]
--INTO #t3
--FROM
--(	SELECT	
--		[SSIS_FileName]
--		,MAX([TimePassed]) AS Duration_in_min
--	--INTO #t3
--	FROM [#t2]
--	GROUP BY [SSIS_FileName]



--SELECT	* 
--INTO	kettler_3b_agg
--FROM	#t3

--SELECT	#t2.*
--		,[#t3].[Duration_in_min]
--		,ROUND(CAST([TimePassed] as FLOAT) / CAST([Duration_in_min] AS FLOAT),3) AS PctComplete
--INTO kettler_3a_fact
--FROM #t2
--LEFT JOIN [#t3] ON [#t2].[SSIS_FileName] = [#t3].[SSIS_FileName]


-----------------------------------------------------------------------
-- 2018-03-02 UPDATE: Null Werte einbauen, damit kont. Kurven entstehen
-- this doesnt work well, better try via PowerPivot Options to fill in zero days
/*
SELECT 
	f.SSIS_FileName,
	d.Date_Id as Date,
	f.Duration_in_min,
	f.Avg_Pulse,
	f.Avg_Power,
	f.Avg_RPM 
INTO #t3
FROM 
	(SELECT Date_Id FROM kettler_date_dim) d
LEFT JOIN 
	(SELECT * FROM kettler_3b_agg) f
ON d.Date_Id=f.Date
-- NULLWERTE -> 0 zur Auswertung. Pulse nicht, hier kommt 0 bereits vor
UPDATE t
SET Duration_in_min = 0, Avg_Power = 0, Avg_RPM = 0
from #t3 t
WHERE SSIS_FileName is null
-- overwrite
TRUNCATE TABLE kettler_3b_agg
INSERT INTO kettler_3b_agg (SSIS_FileName, Date, Duration_in_min, Avg_Pulse, Avg_Power, Avg_RPM)
	SELECT * FROM #t3
*/


--SELECT * 
--INTO dim_date_training 
--FROM meta.dim_Date WHERE Date_Id BETWEEN (SELECT MIN(DAte) FROM kettler_3b_agg) AND (SELECT MAX(DAte) FROM kettler_3b_agg)


end
GO


