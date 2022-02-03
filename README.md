Domain: http://localhost:7000

Url: Domain/Role/Action

Examples: 

- http://localhost:7000/auth/register
- http://localhost:7000/user/profile
- http://localhost:7000/admin/changerole
- http://localhost:7000/owner/setmaxroles

Role types:

- Owner (the CEO or Co-Founder of the app)
- Admin (the Administrator who maintain the app)
- Member ([NEW] a user who has paid a monthly fee to see more functionalities)
- User (a simple user)
- Auth (guest without profile)

Action types:

- Register
- Login
- Profile
- Logout
- Update
- Change
- ResetPassword
- Delete
- DeleteAccount
- DeleteUser
- DeleteUserAccount
- ChangeRole
- GetUsers
- GetRoles
- GetEntries
- SetMaxRoles


|Endpoints|Owner|Admin|Member|User|Guest|
| :- | :-: | :-: | :-: | :-: | :-: |
|Register|**No**|**No**|**No**|**No**|**Yes**|
|Login|**No**|**No**|**No**|**No**|**Yes**|
|Profile|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Logout|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Update|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Change (Email/Password/Username)|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|ResetPassword|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Delete|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|DeleteAccount|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|DeleteUser|**Yes**|**Yes**|**No**|**No**|**No**|
|DeleteUserAccount|**Yes**|**Yes**|**No**|**No**|**No**|
|ChangeRole|**Yes**|**Yes**|**No**|**No**|**No**|
|GetUsers|**Yes**|**Yes**|**No**|**No**|**No**|
|GetRoles|**Yes**|**Yes**|**No**|**No**|**No**|
|GetEntries|**Yes**|**Yes**|**No**|**No**|**No**|
|SetMaxRoles|**Yes**|**No**|**No**|**No**|**No**|


