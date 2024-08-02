# 🚀 Slot service

Welcome to **SlotService**! This RESTful API manages appointment slots for
patients and includes a built-in Swagger UI for easy API interaction.

## 📋 Table of Contents

- [🚀 Slot service](#-slot-service)
  - [📋 Table of Contents](#-table-of-contents)
  - [📖 Description](#-description)
  - [🚀 Getting Started](#-getting-started)
  - [🛠 Prerequisites](#-prerequisites)
  - [⚙️ Installation](#️-installation)
    - [Clone the Repository](#clone-the-repository)
    - [Restore Dependencies](#restore-dependencies)
    - [Build the solution](#build-the-solution)
  - [🏃 Running the Application](#-running-the-application)
  - [🌐 Using Swagger](#-using-swagger)
  - [📝 Change default data](#-change-default-data)

## 📖 Description

Doctors offer slots. A slot is a period of time which the patient could ask for
a visit. The doctor defines a slot duration (for example, 20 minutes) and
determines the work period (from 8 am to 1 pm, for example). The doctor expects
that the patient will be able to see available slots and book an appointment (slot).

## 🚀 Getting Started

Follow these instructions to set up Slot service on your local machine.

## 🛠 Prerequisites

Make sure you have the following installed:

- **.NET 8 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git**: [Download here](https://git-scm.com/downloads)

## ⚙️ Installation

Open a terminal and run following commands 

### Clone the Repository

```sh
git clone https://github.com/AbelSanchezTuya/SlotService.git
cd SlotService
```

### Restore Dependencies

```sh
dotnet restore
```

### Build the solution

```sh
dotnet build
```

## 🏃 Running the Application

Start the application with:

```sh
dotnet run --project SlotService.API.REST
```

By default, the application will run at [https://localhost:7188](https://localhost:7188).

## 🌐 Using Swagger

Swagger provides an interactive interface to explore and test the API. The
application should automatically open a browser with swagger. If this is not the
case, once the application is running:

1. Open Your Browser

2. Navigate to Swagger UI

Go to [https://localhost:7188/swagger/index.html](https://localhost:7188/swagger/index.html) to view the API documentation and test endpoints.

## 📝 Change default data

The application includes some default weeks available for booking and few busy slots. If you need to create new weeks
for testing purposes, follow these steps:

1. Navigate to the file `SlotService.Storage\Helper\DevData.json`.
2. Modify the file as needed.

**Remarks:**

- Weeks are added sequentially, with the first week being the current week.
- This data is not validated, so ensure consistency and validity yourself.
  This kind of invalid data scenarios are not covered by the application.


---

Enjoy using the Slot service! 🎉

---

