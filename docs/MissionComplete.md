MissionComplete

# Product Requirements Document (PRD) for Off-Season Challenge Tracking App

## 1. Overview
### 1.1 Purpose
The Off-Season Challenge Tracking App enables coaches to assign challenges and workouts to their players, allowing the players and their coach to track progress. Players log completed challenges, and coaches monitor activity using a matrix-style dashboard.

### 1.2 Goals
- Provide a structured way for coaches to assign off-season workouts.
- Allow players to log completed workouts.
- Display a clear progress-tracking system for both players and coaches.
- Ensure robust error handling, logging, and detailed comments in code.

## 2. Features & Functionalities

### 2.1 User Roles
- **Coach:** Can create, assign, and track challenges for players and manage multiple teams.
- **Player:** Can view assigned challenges, mark them as completed, and view progress.
- **Admin:** Manages user access and resolves issues.

### 2.2 Core Functionalities
#### 2.2.1 Authentication & User Management
- User authentication via email/password or OAuth (Google, Facebook, etc.).
- Role-based access control (RBAC) for different user types.
- Password reset and account recovery.

#### 2.2.2 Team & Challenge Management
- Coaches can create and manage multiple teams.
- Each team has its own set of players and challenges.
- Coaches create and assign challenges with:
  - Name (e.g., "Run 5K", "Shoot 100 free throws").
  - Type (Cardio, Strength, Skill-based, etc.).
  - Frequency (Daily, Weekly, Custom).
  - Start and end dates.
  - Notes or instructions.
- Players view assigned challenges and log completions.

#### 2.2.3 Progress Tracking
- **Matrix-style visualization:**
  - Players listed on one axis, time on the other.
  - Each challenge is marked when completed.
- Individual player progress views.
- Reports and statistics for coaches.
- Coaches can view progress per team or per player.

#### 2.2.4 Notifications
- Automatic reminders for upcoming challenges.
- Notifications for overdue challenges.
- Summary reports to coaches.

#### 2.2.5 Error Handling & Logging
- Validation of input data.
- Error logs for failed operations.
- Logging of key actions (challenge creation, completion, etc.).

## 3. System Architecture
### 3.1 Tech Stack
- **Frontend:** React + Tailwind CSS
- **Backend:** .NET 8 Core Web API (C#)
- **Database:** SQL Server @10.0.0.201 (using Entity Framework Core)
- **Hosting:** Docker
- **Configuration:** User Secrets for development, Environment Variables for production

### 3.2 Data Models
#### 3.2.0 User
- **Properties:**
  - Id: Unique identifier
  - Email: User's email address
  - PasswordHash: Hashed password using BCrypt
  - Role: Enum (Player, Coach, Admin)
  - CreatedAt: Timestamp of user creation

#### 3.2.1 Team
- **Properties:**
  - Id: Unique identifier
  - Name: Team name
  - Description: Optional team description
  - CreatedAt: Timestamp of team creation
  - Players: Collection of players in the team
  - Challenges: Collection of challenges assigned to the team

#### 3.2.2 Player
- **Properties:**
  - Id: Unique identifier
  - Name: Player's full name
  - Email: Player's email address
  - PhoneNumber: Optional contact number
  - CreatedAt: Timestamp of player creation
  - TeamId: Foreign key to Team
  - CompletedChallenges: Collection of completed challenges

#### 3.2.3 Challenge
- **Properties:**
  - Id: Unique identifier
  - Name: Challenge name
  - Description: Challenge details and instructions
  - Type: Enum (Cardio, Strength, SkillBased, Other)
  - Frequency: Enum (Daily, Weekly, Custom)
  - StartDate: Challenge start date
  - EndDate: Challenge end date
  - CreatedAt: Timestamp of challenge creation
  - TeamId: Foreign key to Team
  - Completions: Collection of challenge completions

#### 3.2.4 ChallengeCompletion
- **Properties:**
  - Id: Unique identifier
  - CompletedAt: Timestamp of completion
  - Notes: Optional completion notes
  - PlayerId: Foreign key to Player
  - ChallengeId: Foreign key to Challenge

### 3.3 API Endpoints
#### 3.3.1 Authentication
The application uses JWT (JSON Web Tokens) for authentication. Users receive a token upon successful login, which must be included in subsequent requests.

#### 3.3.2 JWT Configuration
```json
{
  "Jwt": {
    "Key": "<your-secret-key>",
    "Issuer": "https://missioncomplete.com",
    "Audience": "https://missioncomplete.com"
  }
}
```

#### 3.3.3 Authentication Flow
1. User submits email and password to `/api/auth/login`
2. Server validates credentials and returns JWT token
3. Client includes token in Authorization header for subsequent requests
4. Server validates token and authorizes requests based on user role

#### 3.3.3 Development Endpoints
The application includes development-only endpoints for initial setup:

- POST `/api/auth/dev/create-admin`
  - Creates an admin user with the specified email and password
  - Should be disabled in production
  - Example request:
    ```json
    {
      "email": "admin@example.com",
      "password": "secure_password"
    }
    ```

#### 3.3.2 Team & Challenge Management
```