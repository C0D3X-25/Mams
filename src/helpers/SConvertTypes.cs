namespace Mams.src.helpers;

public static class SConvertTypes {

    public static int tryConvertToID(int id) {
        if (id < 0) {
            throw new ArgumentOutOfRangeException();
        }
        return id;
    }


    public static int tryConvertToID(string id) {
        if (!isInteger(id)) {
            throw new FormatException();
        }
        return tryConvertToID(Convert.ToInt32(id));
    }


    public static int tryConvertToIntegerMoreThanZero(int value) {
        if (value <= 0) {
            throw new ArgumentOutOfRangeException();
        }
        return value;
    }


    public static int tryConvertToIntegerMoreThanZero(string value) {
        if (!isInteger(value)) {
            throw new FormatException();
        }
        return tryConvertToIntegerMoreThanZero(Convert.ToInt32(value));
    }


    public static bool isInteger(string value) {
        try {
            Convert.ToInt32(value);
            return true;
        }
        catch {
            return false;
        }
    }
}
