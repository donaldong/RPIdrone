import mysql.connector

class Database:
    __name = "drone"
    __user = "root"
    __password = "root"
    __host = "localhost"

    __quries_create_table = {}
    __quries_create_table["motor"] = (
        "CREATE TABLE `motor` ("
        "  `id` int(11) NOT NULL AUTO_INCREMENT,"
        "  `name` varchar(16) NOT NULL,"
        "  `pulse_width` int(11) NOT NULL,"
        "   PRIMARY KEY (`id`))"
    )

    @staticmethod
    def connect():
        return mysql.connector.connect(
            user=Database.__user, password=Database.__password, 
            host=Database.__host, database=Database.__name)
    
    @staticmethod
    def create_tables():
        cnx = Database.connect()
        cursor = cnx.cursor()
        for table in Database.__quries_create_table:
            query = Database.__quries_create_table[table]
            print(query)
            cursor.execute(query)
        cnx.close()

