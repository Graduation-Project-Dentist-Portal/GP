Tables:

- user

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid | id | Primary Key |
	| String | first_name |  |
	| String | last_name |  |
	| String | password_hash | |
	| Guid | refresh_token_id | Foreign Key => (refresh_token.id) |
	| String | profile_picture | |
	| String | password_hash | |
	| String | role | |
	| Boolean | is_active | |

- refresh_token

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | token |  |
	| TimeStamp | time_created |  |
	| TimeStamp | time_expires |  |
	| Boolean | is_active | |

- tool_for_sale

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | name |  |
	| String | description |  |
	| Float | tool_price |  |
	| Guid | seller_id | Foreign Key => (user.id) |
	| Boolean | is_active | |

- job

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | name |  |
	| String | description |  |
	| Float | salary |  |
	| Guid | owner_id | Foreign Key => (user.id) |
	| Boolean | is_active | |

- post

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | description |  |
	| Guid | user_id | Foreign Key => (user.id) |
	| Boolean | is_active | |

- picture

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | picture_path |  |
	| Guid | owner_id | Foreign Key => (post.id) or Foreign Key => (tool_for_sale.id) |
	| Boolean | is_active | |

- medical_case

	| Field Type | Field Name | Keys
	| --- | --- | --- |
    | Guid |id | Primary Key |
	| String | description |  |
	| String | patient_name |  |
	| String | patient_phone |  |
	| String | patient_age |  |
	| Guid | owner_id | Foreign Key => (user.id) |

Relationships:

| Type | Tables involved 
| --- | --- |
| One to One | user to refresh_token |
| One to Many | user to tool_for_sale |
| One to Many | user to post |
| One to Many | user to job |
| One to Many | post to picture |
| One to Many | tool_for_sale to picture |
| One to Many | user to medical_case |

Notes:
-	user identity card still messing
-	jobs contact info and the job owner still missing