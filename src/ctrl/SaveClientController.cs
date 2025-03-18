using Mams.commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mams.src.ctrl;

public class SaveClientController {

    public RelayCommand m_save_command;
    public RelayCommand m_abort_command;

    public SaveClientController() {
        m_save_command = new(execute => saveClient(), can_execute => true);
        m_abort_command = new(execute => abortClient(), can_execute => true);
    }

    private void saveClient() {
        MessageBox.Show("Client saved");
    }

    private void abortClient() {
        MessageBox.Show("Client aborted");
    }
}
