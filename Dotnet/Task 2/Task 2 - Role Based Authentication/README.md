# 🍔🍽️ Food Ordering System API

The Food Ordering System API manages user authentication, account creation, login, profile management, cart operations, and order handling for a food ordering system.

## 📜 Overview

This API encompasses multiple functionalities, divided across several controllers. Here's an outline of the available endpoints:

### 🗃️ Account Controller
- `GET /api/Account/IsEmailAvailable`: Check email availability.
- `GET /api/Account/IsValidUser`: Validate user existence.
- `POST /api/Account/Signup`: Register a new user.
- `POST /api/Account/Login`: Authenticate a user and generate JWT tokens.
- `GET /api/Account/UserProfile/{userId}`: Retrieve user profile information.
- `POST /api/Account/UserProfile`: Update user profile details.
- `POST /api/Account/Logout`: Logout a user session.
- `POST /api/Account/VerifyOtp`: Verify an OTP for a user.
- `POST /api/Account/ResendOtp`: Resend an OTP to a user.

### 🛒 Cart Controller
- `GET /api/Cart`: Fetch items in the cart.
- `POST /api/Cart`: Add items to the cart.
- `POST /api/Cart/{itemId}/increase`: Increase the quantity of a cart item.
- `POST /api/Cart/{itemId}/decrease`: Decrease the quantity of a cart item.
- `DELETE /api/Cart/{itemId}`: Remove an item from the cart.

### 📦 Order Controller
- `POST /api/Order/Checkout`: Process orders and payments.
- `POST /api/Order/payment`: Execute payments via Razorpay.
- `GET /api/Order/OrderHistory`: Retrieve order history for a user.

### 📝 Logs Controller
- `POST /api/logs`: Log messages with varying log levels.

## 🛠️ Installation and Setup

1. Clone this repository.
2. Configure essential parameters in the `appsettings.json` file.
3. Run the application using your preferred method (e.g., Visual Studio, command line).

<<<<<<< HEAD
=======
## 📦 Dependencies and Testing

This API is built on ASP.NET Core and incorporates various dependencies such as Entity Framework Core, Serilog for logging, and integration with the Razorpay Payment Gateway. Thorough unit testing is available in the `/Tests` directory.
>>>>>>> a51f8219ff56595c85cc2567e53efd727ee61c86
