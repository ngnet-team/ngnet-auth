NgNet Authorization [Demo version]

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
- GetEntries
- SetMaxRoles

Account properties:

`	`Required: Email, Username, Password

`	`Optional: FirstName, LastName, MiddleName, Gender, Age,

Address: {Country, City, Str},

Contact: {Mobile, Email, Website, Facebook, Instagram, TikTok, Youtube,    Twitter},

`	`Base: CreatedOn, ModifiedOn, DeletedOn, IsDeleted




|Actions Permission|Owner|Admin|Member|User|Guest|
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


|Auth Endpoints|Method|JWT|Body|Response|
| :- | :-: | :-: | :-: | :-: |
|Register|POST|**No**|Email, Username, Password, RepeatPassword + Optional proparties|Successful message|
|Login|POST|**No**|Username, Password|Successful message|
||||||
|User Endpoints|Method|JWT|Body| |
|Profile|GET|**Yes**|` `- |Email, Username, CreatedOn, All Optional Properties|
|Logout|GET|**Yes**|` `- |Successful message|
|Update|POST|**Yes**|An account optional property|Successful message|
|Change (Email/Password/Username)|POST|**Yes**|An account required property|Successful message|
|ResetPassword|GET|**Yes**|` `- |Successful message +  Emailed|
|Delete|GET|**Yes**|` `- |Successful message|
|DeleteAccount|GET|**Yes**|` `- |Successful message|
||||||
|Member Extends User Endpoints|Method|JWT|Body| |
|No more actions|` `-|` `-|` `-|` `-|
||||||
|Admin Extends Member Endpoints|Method|JWT|Body| |
|Profile|GET|**Yes**|` `- |Id, RoleName, Entries, BaseModels|
|Update|POST|**Yes**|Id - update other account (optional)|Successful message|
|Change (Email/Password/Username)|POST|**Yes**|Id - update other account (optional)|Successful message|
|DeleteUser|POST|**Yes**|Id|Successful message|
|DeleteUserAccount|POST|**Yes**|Id|Successful message|
|ChangeRole|POST|**Yes**|Id, RoleName|Successful message|
|GetUsers|GET|**Yes**|` `- |An array of profile information|
|GetRoles|GET|**Yes**|` `- |An array of [ Id, Name, MaxCount, BaseModels ]|
|GetEntries|GET|**Yes**|` `- |An array of entries [ UserId, Username, Login, CreatedOn ]|
||||||
|Owner Extends Admin Endpoints|Method|JWT|Body| |
|SetRoleCounts|POST|**Yes**|Name, MaxCount|Successful message|

Constraints:

- User can modify his/her account or lower role one.
