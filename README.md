# Environmental Monitor Backend System

## Overview

This project is a **C# backend environmental monitoring system** that receives indoor environmental sensor data, validates it, enriches it with outdoor weather data, and generates structured reports.

The system is designed to simulate a real IoT monitoring backend and demonstrates networking, JSON processing, scheduling, logging, and modular software architecture.

The application performs the following tasks:

* Receives indoor sensor data from a Raspberry Pi over UDP
* Validates and parses incoming JSON messages
* Retrieves outdoor weather data from the OpenWeather API
* Generates human-readable and JSON reports
* Automatically generates reports on a scheduled interval
* Creates summary reports for user-specified date ranges
* Logs all system events to daily log files

---

# System Requirements

* .NET 8 (or .NET 7)
* Internet connection (for OpenWeather API)
* Raspberry Pi sending valid JSON sensor data
* OpenWeather API Key
* Visual Studio

---

# Expected Input

## From Raspberry Pi (UDP Port 11000)

The Raspberry Pi must send valid JSON in the following format:

```json
{
  "temperatureF": 69.5,
  "temperatureC": 20.8,
  "humidity": 41.2
}
```

If the JSON message is malformed or contains invalid values, the message will be rejected and logged.

---

# How to Run the Program

1. Open the solution in **Visual Studio**.

2. Build the project.

3. Press **F5** to run the program.

4. Use the console menu:

```
1 - Start Server
2 - Generate Summary Report
3 - Exit
```

### Option 1 — Start Server

Starts the environmental monitoring server and listens for incoming sensor data on UDP port **11000**.

### Option 2 — Generate Summary Report

Generates a summary report based on previously generated reports within a user-specified date range.

### Option 3 — Exit

Stops the application.

---

# Output Files

## Scheduled Reports

Each scheduled report generates two files:

* Report_YYYY-MM-DD_HH-MM-SS.txt
* Report_YYYY-MM-DD_HH-MM-SS.json

Reports are organized by date:

```
Reports/
   YYYY-MM-DD/
      Report_YYYY-MM-DD_HH-MM-SS.txt
      Report_YYYY-MM-DD_HH-MM-SS.json
```

---

# Summary Reports

Summary reports are generated when the user selects **Option 2**.

Each summary report produces:

* Summary_YYYY-MM-DD_HH-MM-SS.txt
* Summary_YYYY-MM-DD_HH-MM-SS.json

These files are also saved inside the **Reports** directory.

---

# Logging System

The system records operational events in log files.

A new log file is created each day:

```
Logs/
   YYYY-MM-DD/
      log.txt
```

Log levels used:

* INFO
* WARNING
* ERROR

The logs include:

* Server start events
* Sensor message reception
* Invalid data warnings
* Weather API failures
* Report generation events
* System exceptions

---

# Error Handling

The system includes exception handling for:

* JSON parsing errors
* Invalid sensor values
* Weather API failures
* File I/O errors
* Scheduler exceptions

Errors are recorded in the log files and do not terminate the program.

---

# Troubleshooting

## Weather API Not Working

* Verify the API key is correct.
* Ensure the internet connection is active.
* Wait 5–10 minutes after creating a new API key.

## No Reports Being Generated

* Confirm the Raspberry Pi is sending valid JSON.
* Check the Logs folder for error messages.
* Ensure the scheduler interval is configured correctly.

## JSON Parsing Errors

* Ensure property names use **double quotes**.
* Use `json.dumps()` on the Raspberry Pi to produce valid JSON.

---

# System Architecture

```
Raspberry Pi Sensor
        │
        ▼
   UDP Server
        │
        ▼
   JSON Parser
        │
        ▼
 Weather API Service
        │
        ▼
  Report Generator
        │
        ▼
    Scheduler
        │
        ▼
     Logger
```

---

# Skills Demonstrated

This project demonstrates several software engineering concepts:

* Network programming using UDP
* JSON serialization and deserialization
* REST API integration
* File system operations
* Scheduled background processing
* Logging system design
* Exception handling
* Modular software architecture

---

# Author

**Punoi Vilaivanh**

Computer Science Student
Capstone Programming Project
