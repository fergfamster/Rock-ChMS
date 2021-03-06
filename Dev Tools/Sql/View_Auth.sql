
SELECT  
       a.[Id]
      ,et.Name as EntityTypeName
      ,[EntityId]
      ,[Order]
      ,[Action]
      ,[AllowOrDeny]
      ,[SpecialRole]
      ,[PersonId]
      ,g.Name as GroupName
      ,a.[Guid]
  FROM [Auth] a
  left outer join EntityType et on et.Id = a.EntityTypeId
  left outer join [Group] g on g.Id = a.GroupId
  order by et.Name, a.[Order]