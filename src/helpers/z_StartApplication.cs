using Mams.src.databaseConnections;

namespace Mams.src.helpers;

public static class z_StartApplication {
    
    public static void launchServices() {
        SQLConnectionModel _m_sqlConnectionModel = new();
        _m_sqlConnectionModel.startMysqlService();

        var connection = _m_sqlConnectionModel.openConnection();
        if (connection == null) {
            return;
        }
    }
    
}
