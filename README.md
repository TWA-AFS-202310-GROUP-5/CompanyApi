# RESTful API Comprehensive Exercise

## Business Requirement

* AC1: As a user, I can add a company if its name no same to any existing company
* AC2: As a user, I can obtain all company list
* AC3: As a user, I can obtain an existing company  
* AC4: As a user, I can obtain X(page size) companies from index of Y(page index start from 1)
* AC5: As a user, I can update basic information of an existing company
* AC6: As a user, I can add an employee to a specific company
* AC7: As a user, I can delete a specific employee under a specific company.
* AC8: As a user, I can obtain list of all employee under a specific company
* AC9: As a user, I can update basic information of a specific employee under a specific company
* AC10: As a user, I can delete a specific company, and all employees belong to this company should also be deleted

###### Company contains the following information:
```
* `id`: The *company id* is a non-empty `String` representing the unique ID for a company.
* `name`: The name of the company.

```
###### Employee contains the following information:

```
* `id`: The *employee id* is a non-empty `String` representing the unique ID for an employee.
* `name`: The name of the employee.
* `salary`: The salary of the employee.
```

## Practice Requirement
- Fork this repository
- Design above requirement with RESTful API (Resource and corresponding Http Verb)
- Implement the api test
- Implement api
- write all api design in apis.http 
 
