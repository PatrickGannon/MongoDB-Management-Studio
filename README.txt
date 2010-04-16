This project is very new, more to come... hopefully.  MongoDB client based on WPF/MVVM Light, using MongoDB/C# driver.

Query syntax is as follows:
{collection}:{query}
where {collection} is the name of your collection and {query} is a javascript condition ($where clause)
eg:
mycollection:this.myfield == 'somevalue'

You can also enter just a collection name to show all the data in that collection.
