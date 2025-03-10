using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.items;

public abstract class ABaseItem {
    protected int tryConvertToID(int id) {
        if (id < 0) {
            throw new ArgumentOutOfRangeException();
        }
        return id;
    }


    protected int tryConvertToID(string id) {
        if (!isInteger(id)) {
            throw new FormatException();
        }
        return tryConvertToID(Convert.ToInt32(id));
    }


    protected int tryConvertToIntegerMoreThanZero(int value) {
        if (value <= 0) {
            throw new ArgumentOutOfRangeException();
        }
        return value;
    }


    protected int tryConvertToIntegerMoreThanZero(string value) {
        if (!isInteger(value)) {
            throw new FormatException();
        }
        return tryConvertToIntegerMoreThanZero(Convert.ToInt32(value));
    }


    protected bool isInteger(string value) {
        try {
            Convert.ToInt32(value);
            return true;
        }
        catch {
            return false;
        }
    }
}
