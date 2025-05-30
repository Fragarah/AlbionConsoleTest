# Albion Online Market Tracker (Console Version)

This is a console application for tracking item prices in Albion Online marketplace. It allows you to monitor prices of specific items in different locations and keep track of price history.

## Features

- Import items from the game database
- Track specific items in chosen locations
- View current prices of tracked items
- Update prices automatically
- View price history for tracked items
- Email notifications for price changes (requires configuration)

## Setup

1. Make sure you have .NET 8.0 SDK installed
2. Clone the repository
3. Navigate to the project directory
4. Copy `Data/mailconfig.example.json` to `Data/mailconfig.json` and update it with your email settings:

```json
{
  "SmtpServer": "your.smtp.server",
  "Port": 587,
  "SenderEmail": "your@email.com",
  "SenderPassword": "your_password",
  "EnableSsl": true
}
```

5. Run the application:
```bash
dotnet run
```

## Usage

The application provides a simple console interface with the following options:

1. Import items - Import items from the Albion Online database
2. Track new item - Add a new item to track in a specific location
3. View tracked items - See all items you're currently tracking and their latest prices
4. Update prices - Manually update prices for all tracked items
5. View price history - See the price history for a specific tracked item
6. Exit - Close the application

## Data Storage

The application uses SQLite for data storage. The database file (`albion.db`) will be created automatically in the application directory when you first run the application.

## Dependencies

- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Microsoft.Extensions.Logging.Debug
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting

## Contributing

Feel free to submit issues and enhancement requests! 