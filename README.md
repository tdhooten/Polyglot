# Polyglot

A simple Blazor app which converts libraries of saved links between the native formats of various read-it-later services. Currently supports [GoodLinks](https://goodlinks.app/), [Instapaper](https://www.instapaper.com/), [Omnivore](https://omnivore.app/), [Raindrop.io](https://raindrop.io/), and [Readwise Reader](https://readwise.io/read).

## Instructions

### Installation

#### Docker
Ensure you have Docker installed and running on your system, then clone into the repo and firdt build the app by running `docker build . -t polyglot`, then start the app by running `docker run -p 8080:8080 polyglot`. Finally, navigate to `http://localhost:8080` in your browser.

#### Native
If you wish to build the app natively rather than using Docker, ensure that the latest [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) is installed on the host system, then clone into the repo and execute `dotnet run --project Polyglot` from the project directory. Navigate to the URL indicated in the `dotnet run` command output.

### Usage
After navigating to the local URL, you will be presented with a simple interface allowing you to choose the input format (i.e. your current read-it-later service), the output format (i.e. the read-it-later service you wish to import your library into), and finally the input file itself. After uploading your input file, the exported result of the format conversion process will immediately be available in the default download directory of your current browser.

**Please be aware that each read-it-later service has its own particular limitations**. These are detailed below, along with instructions on how to properly import the file provided by this app.

Finally, **this tool does not transfer highlights between read-it-later services**. If you wish to preserve your highlights when migrating away from a service, please back them up separately.

## GoodLinks

### GoodLinks Limitations

GoodLinks supports many the features of other read-it-later services supported by this app, with the notable exception of folders. In addition, GoodLinks does not use a specific "archived" flag internally; rather, it presents links as "read" to users based on whether the link as a "read at" time. Therefore when GoodLinks is the target export format, all archived links from other apps will be marked as having been "read at" whatever time the format conversion took place. In practice this should not be an issue at all, unless you have an existing GoodLinks library which you are adding to; if this is the case, all imported links in an archived state will appear to have been "read" more recently than those already in GoodLinks at the time of the import.

### GoodLinks Import Instructions

Navigate to `File->Import->GoodLinks` (on macOS) or `Settings->Import->GoodLinks` (on iOS/iPadOS) and upload the "goodlinks.json" file provided by this app. Because this process is irreversible, **users are highly encouraged to test the file conversion and import process using a dummy account before doing so on their permanent account**.

## Instapaper

### Instapaper Limitations

Instapaper's organizational tools have become quite robust. However, although Instapaper *does* support "Likes" (which corresponds to "Stars" or "Favorites" in other apps), Instapaper's import tool uses the same field for "Liked" as it does for "Archive", and only one value can be assigned to that field per link. Therefore, this tool has the following constraint when **importing** into Instapaper:

* **Stars/favorites on items will be ignored** (users are free to manually mark these links as "Liked" in Instapaper after import)

### Instapaper Import Instructions

Nvigate to the Settings page of the Instapaper web app and scroll down until you reach the Import section, then click the "Import from Instapaper CSV" button and provide the "instapaper.csv" file provided by this app. Because this process is irreversible, **users are highly encouraged to test the file conversion and import process using a dummy account before doing so on their permanent account**.

## Omnivore

Omnivore is no longer an actively developed project; however, this tool will continue to support converting Omnivore exports to other read-it-later formats.

## Raindrop.io

### Raindrop.io Limitations

Raindrop.io has a fairly rich feature set, and supports both tags and collections (i.e. folders). However, it does not currently support a native archived/marked-as-read status (though this can be achieved manually in a manner similar to that detailed below). Finally, while it supports a "Favorite" status, this field is not writable using the import tool. Therefore this tool has made the following choices when **importing** into Raindrop.io:

* Archived/read items will be sent to a Raindrop.io collection called "Archive"
* Unread items will be sent to a Raindrop.io collection called "Inbox"
* **Stars/favorites on items will be ignored** (users are free to manually mark these links as "Favorites" in Raindrop.io after import)

When **exporting** from Raindrop.io, **the collection assigned to each item will be ignored**.

### Raindrop.io Import Instructions

Navigate to the Settings page of the Raindrop.io web app and then choose the Import tab on the sidebar. After uploading the "raindrop.csv" file provided by this app, you will be given the option to either import everything, import only new items, or start from scratch by removing all existing links in your account and retaining only the imported items. Because each of these processes is irreversible, **users are highly encouraged to test the file conversion and import process using a dummy account before doing so on their permanent account**.

## Readwise Reader

### Readwise Reader Limitations

Readwise Reader has many excellent features and organizational tools; however, currently their CSV importer only support URLs, titles, timestamps, and unread/archive status. Therefore this tool currently has the following limitations when **importing** into Readwise Reader:

* **Stars/favorites as well as tags on all items will be ignored**

Readwise has a much more robust export function, which this tool intends to support in the future.

### Readwise Reader Import Instructions

Navigate to the main page of the Reader web app. Click the plus icon on the top right of the sidebar, then choose "Upload" and select the "readwise.csv" file provided by this app. Because this process is irreversible, **users are highly encouraged to test the file conversion and import process using a dummy account before doing so on their permanent account**.
