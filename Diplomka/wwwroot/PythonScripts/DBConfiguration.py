import pyodbc

class DBConnection(object):
    def __new__(cls):
        if not hasattr(cls, 'instance'):
            cls.instance = super(DBConnection, cls).__new__(cls)
        return cls.instance

    def getConnection(self):
        return pyodbc.connect('Driver={ODBC Driver 17 for SQL Server};'
                              'Server=(localdb)\MSSQLLocalDB;'
                              'Database=DBDiplom;')
