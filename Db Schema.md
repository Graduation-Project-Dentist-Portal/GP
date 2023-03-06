Tables:

- Dentist

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | Username |  |
	| String | FirstName |  |
	| String | LastName |  |
	| String | PasswordHash | |
	| Guid | RefreshTokenId | Foreign Key => (RefreshToken.Id) |
	| String | ProfilePicture | |
	| Boolean | Graduated | |
	| String | University | |
	| int | Level | |
	| string | IdentityCardPicture | |
	| string | UniversityCardPicture | |
	| Boolean | IsActive | |
	
- Patient

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | Username |  |
	| String | FirstName |  |
	| String | LastName |  |
	| String | PasswordHash | |
	| Guid | RefreshTokenId | Foreign Key => (RefreshToken.Id) |
	| String | ProfilePicture | |
	| Boolean | IsActive | |

- RefreshToken

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Token |  |
	| TimeStamp | TimeCreated |  |
	| TimeStamp | TimeExpires |  |
	| Boolean | IsActive | |

- ToolForSale

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | ToolName |  |
	| String | Description |  |
	| Float | ToolPrice |  |
	| Guid | SellerId | Foreign Key => (Dentist.Id) |		
	| string | PicturePaths | |
	| Boolean | IsActive | |

- Job

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | JobTitle |  |
	| String | Description |  |
	| Float | Salary |  |
	| Guid | OwnerId | Foreign Key => (Patient.Id) or Foreign Key => (Dentist.Id) |
	| Boolean | IsActive | |

- Post

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Description |  |
	| Guid | UserId | Foreign Key => (Patient.Id) or Foreign Key => (Dentist.Id) |
	| string | PicturePaths | |
	| Boolean | IsActive | |

- MedicalCase

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Description |  |
	| String | PatientName |  |
	| String | PatientPhone |  |
	| int | PatientAge |  |	
	| string | PicturePaths | |
	| string | Diagnosis | |
	| string | CaseStatus | |
	| Guid | DoctorId | Foreign Key => (Dentist.Id) |
	| Guid | AssignedDoctorId | Foreign Key => (Dentist.Id) |
	| TimeStamp | TimeCreated |  |
	| Boolean | IsActive | |

- Clinic

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | Adress |  |
	| String | Name |  |
	| Guid | DoctorId | Foreign Key => (Dentist.Id) |
	| String | ClinicPhone |  |
	| int | OpenTime |  |
	| int | CloseTime |  |
	| string | ClinicDescription |  |
	| string | PicturePaths | |
	| Boolean | IsActive | |

- Feedback

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | Comment |  |
	| String | AiScore |  |
	| Guid | ClinicId | Foreign Key => (Clinic.Id) | 
	| Guid | UserId | Foreign Key => (Patient.Id) | 
	| Boolean | IsActive | |

- FinishedCases

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | DoctorWork |  |
	| Guid | DoctorId | Foreign Key => (Dentist.Id) | 
	| string | BeforePicture | |
	| string | AfterPicture | |
    | Guid | CaseId | Foreign Key => (MedicalCase.Id) |


Relationships:

| Type | Tables involved 
| --- | --- |
| One to One | Dentist/Patient to RefreshToken |
| One to Many | Dentist to toolForSale |
| One to Many | Dentist/Patient to Post |
| One to Many | Dentist to Job |
| One to Many | Dentist to MedicalCase |
| One to Many | Dentist to FinishedCases |
| One to Many | Dentist to Clinic |
| One to Many | Patient to Feedback |

Notes:
-	jobs contact info and the job owner still missing