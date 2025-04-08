
create table FamilyTree(
[Person_Id] int not null IDENTITY,
[Relative_Id] int not null,
[Connection_Type] varchar(50) not null,
)
SET IDENTITY_INSERT dbo.FamilyTree ON

--father
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p.Fathеr_Id,'father'  
from dbo.Person as p
where p.Fathеr_Id is not null

--mother
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p.Mother_Id,'mother'  
from dbo.Person as p
where p.Mother_Id is not null
--brother
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p2.Person_Id,'brother'  
from dbo.Person as p join dbo.Person as p2 on (p.Fathеr_Id= p2.Fathеr_Id or p.Mother_Id=p2.Mother_Id) and p.Person_Id != p2.Person_Id
where p2.Gender='male'
--sister
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p2.Person_Id,'sister'  
from dbo.Person as p join dbo.Person as p2 on (p.Fathеr_Id= p2.Fathеr_Id or p.Mother_Id=p2.Mother_Id) and p.Person_Id != p2.Person_Id
where p2.Gender='female'
--son
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p2.Person_Id,'son'  
from dbo.Person as p join dbo.Person as p2 on p.Person_Id= p2.Fathеr_Id or p.Person_Id=p2.Mother_Id
where p2.Gender='male'
--daughter
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p2.Person_Id,'daughter'  
from dbo.Person as p join dbo.Person as p2 on p.Person_Id= p2.Fathеr_Id or p.Person_Id=p2.Mother_Id
where p2.Gender='female'
--spouse male
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p.Spouѕe_Id ,'spouse male'
from dbo.Person as p
where p.Spouѕe_Id is not null and p.Gender='male'
--spouse female
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select p.Person_Id, p.Spouѕe_Id ,'spouse female'
from dbo.Person as p
where p.Spouѕe_Id is not null and p.Gender='female'

select * from dbo.Person

select * from dbo.FamilyTree
