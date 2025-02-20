# Request for Comments (RFC) - Off-Season Challenge Tracking App

## 1. Introduction
### 1.1 Purpose
This RFC outlines the design, implementation, and potential improvements for the Off-Season Challenge Tracking App. The goal is to establish a robust and scalable system that enables coaches to assign and monitor player progress during the off-season.

### 1.2 Scope
- Coaches assign challenges to players.
- Players track and log completed challenges.
- Matrix-style dashboard for progress monitoring.
- Support for multiple teams per coach.

## 2. Problem Statement
Currently, there is no streamlined way for coaches to track players' off-season activities. Existing solutions are manual, lack real-time tracking, and do not provide a clear visual representation of progress.

## 3. Proposed Solution
Develop a web application with a React frontend and .NET Core backend, utilizing SQL Server for data storage and Azure for deployment.

## 4. Technical Specifications
### 4.1 System Architecture
- **Frontend:** React (Next.js) + Tailwind CSS
- **Backend:** .NET Core Web API (C#)
- **Database:** SQL Server
- **Hosting:** Azure App Services

### 4.2 API Design
#### 4.2.1 Authentication
- Email/password authentication.
- OAuth integration (Google, Facebook).
- JWT-based session management.

#### 4.2.2 Team & Challenge Management
- Coaches create multiple teams.
- Assign challenges to players with:
  - Name (e.g., "Run 5K").
  - Type (Cardio, Strength, Skill-based).
  - Frequency (Daily, Weekly, Custom).
  - Start and end dates.
  - Notes or instructions.
- Players log completed challenges.

#### 4.2.3 Progress Tracking
- **Matrix-style dashboard:**
  - Players on one axis, time on the other.
  - Completed challenges marked.
- Individual player performance view.
- Reports and statistics for coaches.

### 4.3 UI/UX Considerations
- **Coach Dashboard:**
  - Team and challenge management.
  - Progress tracking via matrix visualization.
- **Player Dashboard:**
  - Assigned challenges list.
  - Completion logging.
- **Mobile-first Design:**
  - Responsive UI.
  - Push notifications.

## 5. Edge Cases & Error Handling
### 5.1 Edge Cases
- Player marks challenge complete, then reopens it.
- Coach modifies a challenge already logged.
- Network failure while logging progress.
- Player logs an expired challenge.
- Time zone differences affecting deadlines.
- Overlapping players across multiple teams.

### 5.2 Error Handling
- **Invalid Inputs:** API rejects bad data.
- **Concurrency Issues:** Lock mechanisms prevent duplicate logs.
- **Data Integrity:** Foreign key constraints ensure consistency.

## 6. Deployment & CI/CD
- **Azure Pipelines:** Automate build & deployment.
- **Unit Testing:** xUnit for backend, Jest for frontend.
- **Logging & Monitoring:** Azure App Insights for diagnostics.

## 7. Open Questions
- Should players be able to suggest their own challenges?
- Should challenges support attachments (e.g., proof videos)?
- Should coaches have the ability to set challenge difficulty levels?

## 8. Conclusion
This RFC serves as a foundation for discussion and refinement. Feedback is encouraged to ensure a scalable and effective solution for tracking off-season training.
