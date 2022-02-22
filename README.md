

NgNet Authorization [Demo version]


Description: This is a role-based server authentication API.

ApiKey:

- Server side: it should be stored in appsettings.Development.json file for development purposes.
- Client side: it should be passed through out the X-Api-Key header. 

Domain: http://localhost:7000

Url construction: Domain/Role/Action

Examples: 

- http://localhost:7000/auth/register
- http://localhost:7000/user/profile
- http://localhost:7000/admin/changerole
- http://localhost:7000/owner/setmaxroles

Role types:

- Owner (the CEO or Co-Founder of the app)
- Admin (an Administrator who maintains the app)
- Member [NEW] (a user who has paid a monthly fee to see more functionalities)
- User (a simple user)
- Auth (guest without authentication)

Account properties:

- **Required**: Email: string, Username: string, Password: string
- **Optional**: FirstName: string, LastName: string, MiddleName: string, Gender: string, Age: number?, Address: { Country: string, City: string, Str: string }, Contact: { Mobile: string, Email: string, Website: string, Facebook: string, Instagram: string, TikTok: string, Youtube: string, Twitter: string }
- **Base**: RoleId: string, CreatedOn: date, ModifiedOn: date, DeletedOn: date, IsDeleted: boolean




|Actions Permission|Owner|Admin|Member|User|Auth|
| :- | :-: | :-: | :-: | :-: | :-: |
|Register|**No**|**No**|**No**|**No**|**Yes**|
|Login|**No**|**No**|**No**|**No**|**Yes**|
|ResetPassword|**No**|**No**|**No**|**No**|**Yes**|
|Profile|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Logout|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Update|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Change (Email/Password/Username)|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|Delete|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|DeleteAccount|**Yes**|**Yes**|**Yes**|**Yes**|**No**|
|DeleteUser|**Yes**|**Yes**|**No**|**No**|**No**|
|DeleteUserAccount|**Yes**|**Yes**|**No**|**No**|**No**|
|ChangeRole|**Yes**|**Yes**|**No**|**No**|**No**|
|Users|**Yes**|**Yes**|**No**|**No**|**No**|
|Roles|**Yes**|**Yes**|**No**|**No**|**No**|
|Entries|**Yes**|**Yes**|**No**|**No**|**No**|
|RightsChanges|**Yes**|**Yes**|**No**|**No**|**No**|
|SetMaxRoles|**Yes**|**No**|**No**|**No**|**No**|




|Auth Endpoints|Method|JWT|Body|Response|
| :- | :-: | :-: | :-: | :-: |
|Register|POST|**No**|{ Email: string, Username: string, Password: string + Optional proparties }|Successful message|
|Login|POST|**No**|{ Username: string, Password: string }|Successful message|
|ResetPassword|POST|**No**|{ Email: string }|Successful message + Emailed|
||||||
|User Endpoints|Method|JWT|Body|Response|
|Profile|GET|**Yes**||{ Email: string, Username: string, CreatedOn: string, All Optional Properties }|
|Logout|GET|**Yes**||Successful message|
|Update|POST|**Yes**|An account optional property|Successful message|
|Change|POST|**Yes**|{ Key: Email/Password/Username, Old: string, New: string }|Successful message|
|Delete|GET|**Yes**||Successful message|
|DeleteAccount|GET|**Yes**||Successful message|
||||||
|Member Extends User Endpoints|Method|JWT|Body|Response|
|No more actions|` `-|` `-||` `-|
||||||
|Admin Extends Member Endpoints|Method|JWT|Body|Response|
|Profile|GET|**Yes**||{ Id: string, RoleName: string, BaseModels: {} }|
|Update|POST|**Yes**|{ Id: string } - update other account (optional)|Successful message|
|Change|POST|**Yes**|{ Id: string } - update other account (optional)|Successful message|
|DeleteUser|POST|**Yes**|{ Id: string }|Successful message|
|DeleteUserAccount|POST|**Yes**|{ Id: string }|Successful message|
|ChangeRole|POST|**Yes**|{ Id: string, RoleName: string }|Successful message|
|Users|GET|**Yes**||An array of profile information|
|Roles|GET|**Yes**||An array of [{ Id: string, Name: string, MaxCount: number, BaseModels: {} }]|
|Entries|GET|**Yes**||An array of entries [{ UserId: string, Username: string, Login: boolean, CreatedOn: string }]|
|RightsChanges|GET|**Yes**||An array of rights changes [{ From: string, To: string, Role: string, Date: string }]|
||<p></p><p></p>||||
|Owner Extends Admin Endpoints|Method|JWT|Body|Response|
|SetRoleCounts|POST|**Yes**|{ Name: string, MaxCount: number }|Successful message|

Constraints:

- User can modify his/her account or lower role one.
