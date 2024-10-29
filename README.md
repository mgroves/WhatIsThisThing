# What Is This Thing?

## Overview

**What Is This Thing?** is a demo application showcasing vector search and hybrid search using Couchbase. It is an ASP.NET Core demo eCommerce website with features like semantic visual search and geo-location-based product recommendations.

## Projects

- **WhatIsThisThing.Server**: The main ASP.NET Core application backend with API endpoints like Identify and Browse.
- **WhatIsThisThing.Loader**: Populates Couchbase with sample data, including vector embeddings from Azure Computer Vision. (This is optional: you can add things via the Admin UI if you have access and/or are running the demo locally).
- **WhatIsThisThing.Core**: Contains common services and shared configuration.
- **whatisthisthing.client**: The React frontend for the application. (Written by someone who definitely doesn't have React experience).

## Configuration

The shared backend configuration is stored in `appsettings.json` or `appsettings.Development.json` within the `WhatIsThisThing.Core` project:

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

There is an `appsettings.Development.json.sample` file included. You can use this as a starter example when creating your own `appsettings.Development.json` file.

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
   - Create [Capella account and cluster (there is a developer free tier!)](https://www.couchbase.com/products/capella/) or download, install and start [Couchbase Server](https://www.couchbase.com/downloads/?family=couchbase-server) locally.
   - Create a bucket named `whatisthis`.

3. **Create the vector search index and geospatial index**
   - Access the Couchbase web console.
   - Navigate to the "Indexes" tab.
   - Create a new vector search index on the `whatisthis` bucket. (See below for vector index definition)
   - Create a new geospatial index on the `whatisthis` bucket. (See below for geospatial index definition)
   - ⚠️ **Note:** Search/vector/geospatial indexes can be copy/paste imported: [import with Capella](https://docs.couchbase.com/cloud/search/import-search-index.html) / [import with Server](https://docs.couchbase.com/server/current/search/import-search-index.html)

1. **Configure Azure Computer Vision**
   - Obtain your endpoint and subscription key from the Azure portal.
   - Update `appsettings.json` in the `WhatIsThisThing.Core` project with your Azure Computer Vision credentials.
   - Confirm that you have the [correct permission](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#ai--machine-learning) to use this endpoint.
   - ⚠️ **Note:** If you want to use a different embedding model, no problem: implement `IEmbeddingService` and update the corresponding `AddTransient` in Program.cs 

2. **Load sample data into Couchbase**

   - ⚠️ If you intend to use the Admin UI to add data, you can skip this step.

   ```bash
   cd WhatIsThisThing.Loader
   dotnet run
   ```

3. **Build and run the backend (and frontend will also launch via Vite)**
   
   ```bash
   cd WhatIsThisThing.Server
   dotnet run --launch-profile https
   ```

4. **Access the application**
   - Navigate to `https://localhost:5173/` to use the frontend.
   - Use `https://localhost:7145` for the backend API.

5. **Create SQL++ indexes** (optional)
   - Adding (covering) indexes may help to improve performance.
   - See "Index" section for indexes recommended by Query Advisor.
   - YMMV: better queries/indexes may be possible. This demo is not meant to provide SQL++ writing guideance.

## Usage

### What Is This Thing?
- Upload a picture to identify the item (and other similar-looking items).
- Get item details including price, description, and nearby stores that have the item in stock. If no store near you has the item in stock, it will say "Delivery Only".
- There are some sample photos of items that you can try to see how well the image model recognizes similar images in the Loader project, `images/slightlydifferent` folder.

### Browse Page
- View the entire catalog.
- Filter items based on review scores/prices.

### Stores
- View the list of stores.
- Stores are "name" plus lat/lon.

### Admin Area
- There is an admin area that allows users to add/edit/delete items, stores, and stock.
- As implemented, this area requires OAuth access via Google account, restricted to @couchbase.com email addresses.
- It's meant for non-technical users or users without direct database access to be able to add items for demo purposes at booths, customer meetings, etc.
- To access this area yourself, modify the GoogleOAuth section of appsettings.json + main.jsx GoogleOAuthProvider.

## License

This demo project is licensed under the MIT License.

## Indexes

<details>
<summary>Vector Search Index Definition</summary>

```
{
 "name": "ix_items_imageVector",
 "type": "fulltext-index",
 "params": {
  "doc_config": {
   "docid_prefix_delim": "",
   "docid_regexp": "",
   "mode": "scope.collection.type_field",
   "type_field": "type"
  },
  "mapping": {
   "default_analyzer": "standard",
   "default_datetime_parser": "dateTimeOptional",
   "default_field": "_all",
   "default_mapping": {
    "dynamic": true,
    "enabled": false
   },
   "default_type": "_default",
   "docvalues_dynamic": false,
   "index_dynamic": true,
   "store_dynamic": false,
   "type_field": "_type",
   "types": {
    "_default.Items": {
     "dynamic": false,
     "enabled": true,
     "properties": {
      "imageVector": {
       "enabled": true,
       "dynamic": false,
       "fields": [
        {
         "dims": 1024,
         "index": true,
         "name": "imageVector",
         "similarity": "l2_norm",
         "type": "vector",
         "vector_index_optimized_for": "latency"
        }
       ]
      }
     }
    }
   }
  },
  "store": {
   "indexType": "scorch",
   "segmentVersion": 16
  }
 },
 "sourceType": "gocbcore",
 "sourceName": "whatisthis",
 "sourceUUID": "12efaf77d46c97bcb97a21e0072ed5b0",
 "sourceParams": {},
 "planParams": {
  "maxPartitionsPerPIndex": 1024,
  "indexPartitions": 1,
  "numReplicas": 0
 },
 "uuid": "3766c659c874c8ea"
}
```
</details>

<details>
<summary>Geospatial Search Index Definition</summary>

```
{
 "name": "ix_store_geo",
 "type": "fulltext-index",
 "params": {
  "doc_config": {
   "docid_prefix_delim": "",
   "docid_regexp": "",
   "mode": "scope.collection.type_field",
   "type_field": "type"
  },
  "mapping": {
   "default_analyzer": "standard",
   "default_datetime_parser": "dateTimeOptional",
   "default_field": "_all",
   "default_mapping": {
    "dynamic": true,
    "enabled": false
   },
   "default_type": "_default",
   "docvalues_dynamic": false,
   "index_dynamic": true,
   "store_dynamic": false,
   "type_field": "_type",
   "types": {
    "_default.Stores": {
     "dynamic": false,
     "enabled": true,
     "properties": {
      "geo": {
       "enabled": true,
       "dynamic": false,
       "fields": [
        {
         "index": true,
         "name": "geo",
         "type": "geopoint"
        }
       ]
      }
     }
    }
   }
  },
  "store": {
   "indexType": "scorch",
   "segmentVersion": 15,
   "spatialPlugin": "s2"
  }
 },
 "sourceType": "gocbcore",
 "sourceName": "whatisthis",
 "sourceUUID": "12efaf77d46c97bcb97a21e0072ed5b0",
 "sourceParams": {},
 "planParams": {
  "maxPartitionsPerPIndex": 1024,
  "indexPartitions": 1,
  "numReplicas": 0
 },
 "uuid": "532826a780cd6e4d"
}
```
</details>

<details>

<summary>SQL++ Indexes from Query Advisor Recommendations</summary>

Main query covering index:

```
CREATE INDEX `adv_u_name_name_desc_image_price_rating` ON `whatisthis`.`_default`.`Items`((`u`.`name`) INCLUDE MISSING,`name`,`desc`,`image`,`price`,`rating`)
```

Main query covering index (when filters used):

```
CREATE INDEX `adv_price_rating_image_name_desc_u_name` ON `whatisthis`.`_default`.`Items`(`price`,`rating`,`image`,`name`,`desc`,(`u`.`name`))
```

Subquery covering index:

```
CREATE INDEX `adv_split_meta_id_1_split_meta_id_0_numInStock` ON `whatisthis`.`_default`.`Stock`((split((meta().`id`), "::")[1]),(split((meta().`id`), "::")[0]),`numInStock`)
```

</details>

## Public Deployment

There is currently a public deployment of this demo.

> [!WARNING]
> This demo is running on Capella Free Tier, with Azure Computer Vision free tier (F0), and Basic Azure Web App hosting (B1), with a React front-end written by someone who does not know React that well. It can handle a moderate amount of traffic (I have done some basic [JMeter Testing](whatisthisthing.jmx)). However, if it gets a lot of traffic, it will likely crash. Please don't consider this to be representative of any of the underlying technologies! A true production deployment would use a more robust/paid hosting solution.

That being said, have at it: https://whatisthisthing.azurewebsites.net