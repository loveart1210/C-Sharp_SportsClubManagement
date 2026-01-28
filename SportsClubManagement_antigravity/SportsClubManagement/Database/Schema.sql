-- Create database if not exists
CREATE DATABASE IF NOT EXISTS sports_club_management;

USE sports_club_management;

-- Users table
CREATE TABLE IF NOT EXISTS Users (
    Id VARCHAR(50) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'User',
    AvatarPath TEXT NULL,
    BirthDate DATETIME NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Teams table
CREATE TABLE IF NOT EXISTS Teams (
    Id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT NULL,
    AvatarPath TEXT NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Balance DECIMAL(18, 2) DEFAULT 0,
    JoinCode VARCHAR(20) UNIQUE
);

-- TeamMembers table
CREATE TABLE IF NOT EXISTS TeamMembers (
    Id VARCHAR(50) PRIMARY KEY,
    UserId VARCHAR(50) NOT NULL,
    TeamId VARCHAR(50) NOT NULL,
    Role VARCHAR(50) NOT NULL DEFAULT 'Member',
    JoinDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    FOREIGN KEY (TeamId) REFERENCES Teams (Id) ON DELETE CASCADE,
    UNIQUE KEY idx_team_user (TeamId, UserId)
);

-- Subjects table
CREATE TABLE IF NOT EXISTS Subjects (
    Id VARCHAR(50) PRIMARY KEY,
    TeamId VARCHAR(50) NULL,
    UserId VARCHAR(50) NULL,
    Name VARCHAR(100) NOT NULL,
    Description TEXT NULL,
    ParticipantCount INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TeamId) REFERENCES Teams (Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE
);

-- Sessions table
CREATE TABLE IF NOT EXISTS Sessions (
    Id VARCHAR(50) PRIMARY KEY,
    TeamId VARCHAR(50) NULL,
    UserId VARCHAR(50) NULL,
    SubjectId VARCHAR(50) NULL,
    Name VARCHAR(100) NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Note TEXT NULL,
    IsAttended BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TeamId) REFERENCES Teams (Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    FOREIGN KEY (SubjectId) REFERENCES Subjects (Id) ON DELETE SET NULL,
    INDEX idx_team_start (TeamId, StartTime)
);

-- Attendances table
CREATE TABLE IF NOT EXISTS Attendances (
    Id VARCHAR(50) PRIMARY KEY,
    SessionId VARCHAR(50) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    IsPresent BOOLEAN NOT NULL DEFAULT FALSE,
    Note TEXT NULL,
    RecordedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SessionId) REFERENCES Sessions (Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    UNIQUE KEY idx_session_user (SessionId, UserId)
);

-- FundTransactions table
CREATE TABLE IF NOT EXISTS FundTransactions (
    Id VARCHAR(50) PRIMARY KEY,
    TeamId VARCHAR(50) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Description TEXT NULL,
    ByUserId VARCHAR(50) NOT NULL,
    Type VARCHAR(20) NOT NULL DEFAULT 'Deposit',
    Date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TeamId) REFERENCES Teams (Id) ON DELETE CASCADE,
    FOREIGN KEY (ByUserId) REFERENCES Users (Id) ON DELETE CASCADE
);

-- Notifications table
CREATE TABLE IF NOT EXISTS Notifications (
    Id VARCHAR(50) PRIMARY KEY,
    TeamId VARCHAR(50) NULL,
    ByUserId VARCHAR(50) NOT NULL,
    Title VARCHAR(200) NOT NULL,
    Content TEXT NOT NULL,
    IsSystemNotification BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TeamId) REFERENCES Teams (Id) ON DELETE CASCADE,
    FOREIGN KEY (ByUserId) REFERENCES Users (Id) ON DELETE CASCADE
);

-- Basic Seed Data
INSERT IGNORE INTO
    Users (
        Id,
        Username,
        Password,
        FullName,
        Email,
        Role,
        BirthDate,
        CreatedDate
    )
VALUES (
        '3dc45d06-1b9f-4793-8621-641647249afe',
        'admin',
        'admin123',
        'Admin System',
        'admin@sports.club',
        'Admin',
        '1990-01-01',
        NOW()
    );