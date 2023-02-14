Tables:

- User

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | Id | Primary Key |
	| String | Username |  |
	| String | FirstName |  |
	| String | LastName |  |
	| String | PasswordHash | |
	| Guid | RefreshTokenId | Foreign Key => (RefreshToken.Id) |
	| String | ProfilePicture | |
	| String | Role | |
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
	| Guid | SellerId | Foreign Key => (User.Id) |
	| Boolean | IsActive | |

- Job

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | JobTitle |  |
	| String | Description |  |
	| Float | Salary |  |
	| Guid | OwnerId | Foreign Key => (User.Id) |
	| Boolean | IsActive | |

- Post

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Description |  |
	| Guid | UserId | Foreign Key => (User.Id) |
	| Boolean | IsActive | |

- Picture

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | PicturePath |  |
	| Guid | OwnerId | Foreign Key => (Post.Id) or Foreign Key => (ToolForSale.Id) |
	| Boolean | IsActive | |

- MedicalCase

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Description |  |
	| String | PatientName |  |
	| String | PatientPhone |  |
	| String | PatientAge |  |
	| Guid | DoctorId | Foreign Key => (User.Id) |
	| Boolean | IsActive | |

- Clinic

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Adress |  |
	| Guid | DoctorId | Foreign Key => (User.Id) |
	| String | ClinicPhone |  |

- Feedback

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |Id | Primary Key |
	| String | Comment |  |
	| Guid | ClinicId | Foreign Key => (Clinic.Id) |

Relationships:

| Type | Tables involved 
| --- | --- |
| One to One | User to RefreshToken |
| One to Many | User to toolForSale |
| One to Many | User to Post |
| One to Many | User to Job |
| One to Many | Post to Picture |
| One to Many | ToolForSale to Picture |

Notes:
-	User identity card still messing
-	jobs contact info and the job owner still missing