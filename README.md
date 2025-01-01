# Barber Control System - BarberShop

This project is a **Barber Control System**, designed for managing barbershop operations including services, employees, appointments, and user management. The project was developed as part of a Web Programming course.

---

## **Project Overview**

The system provides the following features:
1. **Barber/Service Definitions:**
   - Allows defining various services for a single barbershop.
   - Services can be edited or deleted as needed.

2. **Employee Management:**
   - Enables the definition of employees who provide the services.
   - Employee working hours are managed, enabling customers to book appointments accordingly.

3. **Appointment System:**
   - Customers can book appointments based on employee availability.
   - Admins can approve appointments and view employee performance via detailed reports.

4. **User Membership and Authorization:**
   - Customers can register and log in to access services.
   - Authorization is implemented to ensure secure access control for admins and users.

---

## **Technologies Used**

- **Framework:** ASP.NET Core MVC
- **Database:** Microsoft SQL Server (via Entity Framework Core)
- **Frontend:** Razor Views (HTML, CSS)
- **Authentication:** ASP.NET Identity
- **Languages:** C#

---

## **Project Structure**

### **Controllers**
- **`AccountController`:** Manages user registration, login, and authentication.
- **`CalisanController`:** Handles employee creation, deletion, and working hour management.
- **`HizmetController`:** Manages services (add, edit, delete, view).
- **`HomeController`:** Displays the homepage.
- **`RandevuController`:** Handles appointment booking, deletion, and approval.

### **Data**
- **`ApplicationDbContext`:** Configures database connections and models. Responsible for managing database tables.

### **Models**
Defines the structure of database tables:
- Employees
- Services
- Appointments
- Others as required by the project.

### **ViewModels**
Combines data from multiple models to provide structured data to views. Simplifies the data flow between controllers and views.

### **Views**
Frontend UI pages that present the system's functionalities to the user. Each controller directs to its respective views.

---

## **Core Functionalities**
1. **Admin Features:**
   - Define and manage barbershop services.
   - Add, edit, or remove employees and their working hours.
   - Approve or reject appointments.
   - View detailed reports on employee performance.

2. **User Features:**
   - Register and log in to book appointments.
   - View available services and employee schedules.

---

## **Limitations**
While the system is functional, the following features were not implemented due to time constraints:
1. **AI Suggestions:**
   - A planned feature for recommending optimal appointment slots based on employee performance and availability.
2. **RESTful API:**
   - The system currently lacks RESTful API endpoints for external integrations.

---

## **How to Run**
1. Clone the repository:
   ```bash
   git clone https://github.com/EmirVardar/BarberShop.git
