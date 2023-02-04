-- find adjacent trainings
IF ISNULL(OBJECT_ID(N'tempdb..#streak'),0) <> 0 DROP TABLE #streak
SELECT	Rank() OVER (Order by d.Date_Id) as Id
		,cast(d.Date_Id as date) as Date_Id
		,cast(a.TrainingDateTime as date) as TrainingDate
  into #streak
  FROM	[kettler].[dim_Date] d 
  LEFT JOIN [kettler].[Trainings] a on cast(d.Date_Id as date) = cast(a.TrainingDateTime as date)

IF ISNULL(OBJECT_ID(N'tempdb..#streak2'),0) <> 0 DROP TABLE #streak2
  select *
  , RANK() over (order by Date_Id) as xDE 
  into #streak2
  from #streak
  where TrainingDate is null

IF ISNULL(OBJECT_ID(N'tempdb..#streak3'),0) <> 0 DROP TABLE #streak3
  select	l.Id,
			l.Date_Id,
			l.Id - r.Id -1 as Streak 
  into #streak3
  from #streak2 l JOIN #streak2 r on l.xDE = r.xDE + 1

  update a set Streak_days = t.Streak
  from [kettler].[Trainings] a JOIN #streak3 t ON cast(a.TrainingDateTime as date) = dateadd(d, -1, t.Date_Id)
 
 