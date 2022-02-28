select * from dbo.Projects 

select * from dbo.Tasks

select [Description], COUNT([Description]) as "Count"
from dbo.Tasks
group by [Description]
having COUNT([Description]) = 1

select * from dbo.GetDescription

select * from dbo.Tasks as t
inner join dbo.Projects as p on t.Id_Project = p.Id

select * from dbo.Tasks as t
left join dbo.Projects as p on t.Id_Project = p.Id
union all
select * from dbo.Tasks as t
right join dbo.Projects as p on t.Id_Project = p.Id

select * from dbo.Tasks as t
full outer join dbo.Projects as p on t.Id_Project = p.Id

exec dbo.InsertProject "Free time"
