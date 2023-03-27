import pyodbc

connection = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};'
                            'Server=(localdb)\MSSQLLocalDB;'
                            'Database=DBDiplom;')

cursor = connection.cursor()
mySQLQuery = ("""
    SELECT * FROM EducationPrCode
""")

cursor.execute(mySQLQuery)
results = cursor.fetchall()
print(results)

connection.close()