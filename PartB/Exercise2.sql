

SET IDENTITY_INSERT dbo.FamilyTree ON


--update spouse female
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select family.Relative_Id, family.Person_Id ,'spouse female'
from dbo.FamilyTree as family
where not exists
(select *
from dbo.FamilyTree as f where f.Person_Id=family.Relative_Id and f.Relative_Id=family.Person_Id and f.Connection_Type='spouse female')
and family.Connection_Type='spouse male'

--update spouse male
insert into dbo.FamilyTree (Person_Id , Relative_Id , Connection_Type)
select family.Relative_Id, family.Person_Id ,'spouse male'
from dbo.FamilyTree as family
where not exists
(select *
from dbo.FamilyTree as f where f.Person_Id=family.Relative_Id and f.Relative_Id=family.Person_Id and f.Connection_Type='spouse male')
and family.Connection_Type='spouse female'

