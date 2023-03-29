import pyodbc
from DBConfiguration import DBConnection
import uuid;


def CreateTable(cursor, typeTable):
    tableName = "Tmp" + typeTable + "TablePoints"
    query = "CREATE TABLE " + tableName + "(Id VARCHAR(36) PRIMARY KEY, Code VARCHAR(15) NOT NULL, Points INT NOT NULL, Year VARCHAR(4) NOT NULL);"

    cursor.execute(query).commit()
    return tableName

def SetOrdersIntoTable(cursor, year, tableName):
    selectQuery = "SELECT " + tableName[3:6] + "(Points), Code FROM ENTScoresByYear WHERE Year = '" + year + "' GROUP BY Code"
    result = cursor.execute(selectQuery).fetchall()

    for row in result:
        idStr, points, code = str(uuid.uuid4()).upper(), row[0], row[1]
        insert = "INSERT INTO " + tableName + "(Id, Points, Code, Year) VALUES('" + idStr + "', " + str(points) + ", '" + code + "', '" + year + "')"
        cursor.execute(insert).commit()


connection = DBConnection().getConnection()

cursor = connection.cursor()


resultYears = cursor.execute("SELECT DISTINCT(Year) FROM ENTScoresByYear").fetchall()

minTableName, avgTableName = CreateTable(cursor, "MIN"), CreateTable(cursor, "AVG")

for rowYears in resultYears:
    year = rowYears[0]
    SetOrdersIntoTable(cursor, year, minTableName)
    SetOrdersIntoTable(cursor, year, avgTableName)
