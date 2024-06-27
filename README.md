# What Is This Thing?

## Overview

**What Is This Thing?** is a demo application showcasing vector search and hybrid search using Couchbase. It is an ASP.NET Core demo eCommerce website with features like semantic visual search and geo-location-based product recommendations.

## Projects

- **WhatIsThisThing.Server**: The main ASP.NET Core application backend with API endpoints like Identify and Browse.
- **WhatIsThisThing.Loader**: Populates Couchbase with sample data, including vector embeddings from Azure Computer Vision.
- **WhatIsThisThing.Core**: Contains common services and shared configuration.
- **whatisthisthing.client**: The React frontend for the application.

## Configuration

The shared backend configuration is stored in `appsettings.json` within the `WhatIsThisThing.Core` project:

```json
{
  "Couchbase": {
    "ConnectionString": "couchbase://localhost",
    "Username": "Administrator",
    "Password": "password",
    "BucketName": "whatisthis",
    "ScopeName": "_default"
  },
  "AzureComputerVision": {
    "Endpoint": "https://<subdomain>.cognitiveservices.azure.com/computervision/",
    "SubscriptionKey": "<key>"
  }
}
```

## Setup Instructions

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- [Couchbase Capella](https://www.couchbase.com/products/capella/) or [Couchbase Server](https://www.couchbase.com/downloads)
- [Azure Account (for Computer Vision)](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/)

### Step-by-Step Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/mgroves/WhatIsThisThing.git
   cd WhatIsThisThing
   ```

2. **Set up Couchbase**
   - Install and start Couchbase Server.
   - Create a bucket named `whatisthis`.

3. **Create the vector search index**
   - Access the Couchbase web console.
   - Navigate to the "Indexes" tab.
   - Create a new vector search index on the `whatisthis` bucket.

4. **Configure Azure Computer Vision**
   - Obtain your endpoint and subscription key from the Azure portal.
   - Update `appsettings.json` in the `WhatIsThisThing.Core` project with your Azure Computer Vision credentials.

5. **Load sample data into Couchbase**
   ```bash
   cd WhatIsThisThing.Loader
   dotnet run
   ```

6. **Build and run the backend (and frontend will also launch via Vite)**
   ```bash
   cd WhatIsThisThing.Server
   dotnet run --launch-profile https
   ```

8. **Access the application**
   - Navigate to `https://localhost:5173/` to use the frontend.
   - Use `https://localhost:7145` for the backend API.

## Usage

### What Is This Thing?
- Upload a picture to identify the item (and other similar-looking items).
- Get item details including price, description, and nearby stores that have the item in stock.

### Browse Page
- View the entire catalog.
- Filter items based on review scores/prices.

## License

This demo project is licensed under the MIT License.
