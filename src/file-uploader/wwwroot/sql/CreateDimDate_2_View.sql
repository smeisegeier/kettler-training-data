CREATE view [kettler].[dim_Date] as
SELECT * FROM [dbo].[dim_Date]
where Date_Id between 
	(select min(cast(TrainingDateTime as date)) from [kettler].[Trainings])
and (select max(cast(TrainingDateTime as date)) from [kettler].[Trainings])
