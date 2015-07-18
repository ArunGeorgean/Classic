Create procedure [pr_GetDepartmentEmployee](
	@DepartmentId bigint 
)
As
Begin

SELECT * FROM EMPLOYEE E
INNER JOIN DEPARTMENT D ON E.EMPLOYEEID = D.DEPARTMENTID
WHERE D.DEPARTMENTID = @DepartmentId;

End


