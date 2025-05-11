# Shared Calendar Diary API

Hey there!

This is a .NET core 8 Web API project built to improve my backend development skills using .NET, EF Core, and JWT authentication. It’s a basic shared calendar/diary app where users can register, log in, and post a single message and a picture per day. Nothing too fancy, just the core essentials to demonstrate CRUD operations, user authentication, and basic database integration.

## Tech Stack
- **.NET Core 8.0** - Core framework
- **Entity Framework Core** - Database ORM
- **SQLite** - Lightweight dev database
- **JWT Authentication** - Secure access control
- **Swagger** - API testing and documentation

## API Endpoints

### **AuthController** - `/api/auth`
- `POST /register` - Register a new user
- `POST /login` - Authenticate and receive a JWT
- `GET /user` - Retrieve the authenticated user data
- `POST /connect-partner` - Connect with a partner user

### **DiaryController** - `/api/diary`
- `POST /entry` - Create or  today’s entry
- `GET /entry/today` - Retrieve today’s entry
- `GET /entry/all` - Retrieve all entries

---

## Project Structure
- `/Controllers` - Handles API requests
- `/Data` - EF Core context and database config
- `/DTOs` - Request/response data transfer objects
- `/Models` - User and DiaryEntry models
- `/Migrations` - Auto-generated EF Core migration files

---

## Next Steps
- Add SignalR for real-time chat 
- Implement JWT refresh tokens
- Add custom error handling middleware
- Add a unit testing suite with xUnit

---

If you’re reviewing this, feel free to hit me up with any questions or feedback.
