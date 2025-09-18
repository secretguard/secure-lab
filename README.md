# SecureLab - Insecure Starter App

This intentionally insecure ASP.NET Core app is for classroom secure coding practice.

It contains insecure patterns:
- plaintext password storage
- raw SQL concatenation
- no server-side validation
- files saved to wwwroot/uploads with original filename
- developer exception page enabled
- plaintext notes
- no security headers or session management

## Run
dotnet restore
dotnet run

## Endpoints
- POST /Account/Register (form: username, password)
- POST /Account/Login (form: username, password)
- GET  /Profile/Notes/{userId}
- POST /Profile/Notes (form: userId, content)
- POST /Files/Upload (multipart/form-data)
