# Pacer: A Personalized Running Analytics Application

## Introduction

Welcome to Pacer, an academic project that aims to offer a comprehensive solution for personalised endurance training plans. This README serves as a guide to understand the structure and setup of the project.

## Objectives

- Provide race time estimates based on various user metrics.
- Provide personalised training plans based on user's self-defined race goals.
- Incorporate real-time weather data for running gear recommendation.
- Ensure robust security measures for data protection.

## Project Structure

### `Pacer.Data`

This layer encapsulates all data-related functionalities, including:

- **Entity Models**: Defines the data models for database operations.
- **User Service**: Responsible for user authentication and authorization (`UserServiceDb`).
- **Email Service**: Handles email operations (`SmtpMailService`). (Currently not in use due to University Policies)
- **Security**: Manages password hashing (`Pacer.Data.Security.Hasher`).

### `Pacer.Test`

Dedicated to unit testing and other testing functionalities, discussed in detail in the "Testing and Evaluation" chapter of the dissertation.

### `Pacer.Web`

Implements the MVC pattern for the web interface:

- **Controllers**: Manages application flow.
- **Helpers**: Utility functions to assist controllers and views.
- **Models**: View models for the user interface.
- **Dependency Injection**: Configurations outlined in `Program.cs`.

## Further Reading

For a comprehensive understanding of the methodologies employed, refer to the "Design" chapter in the dissertation.

## Licensing

The project is conducted under academic guidelines and is not licensed for external use.

## Access

Project is currently live at www.pacer.page
