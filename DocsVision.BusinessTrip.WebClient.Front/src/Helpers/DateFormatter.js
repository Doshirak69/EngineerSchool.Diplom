var DateFormatter = /** @class */ (function () {
    function DateFormatter() {
    }
    DateFormatter.toDisplayDateTime = function (dateTimeString) {
        if (!dateTimeString)
            return "—";
        try {
            var dt = typeof dateTimeString === 'string'
                ? new Date(dateTimeString)
                : dateTimeString;
            if (isNaN(dt.getTime()))
                return "—";
            var day = dt.getDate().toString().padStart(2, '0');
            var month = (dt.getMonth() + 1).toString().padStart(2, '0');
            var year = dt.getFullYear();
            var hours = dt.getHours().toString().padStart(2, '0');
            var minutes = dt.getMinutes().toString().padStart(2, '0');
            return "".concat(day, ".").concat(month, ".").concat(year, " ").concat(hours, ":").concat(minutes);
        }
        catch (_a) {
            return String(dateTimeString);
        }
    };
    DateFormatter.toAPIFormat = function (date) {
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            throw new Error("Некорректная дата");
        }
        var year = date.getFullYear();
        var month = (date.getMonth() + 1).toString().padStart(2, '0');
        var day = date.getDate().toString().padStart(2, '0');
        return "".concat(year, "-").concat(month, "-").concat(day);
    };
    DateFormatter.toShortDate = function (date) {
        if (!date)
            return "—";
        try {
            var dt = typeof date === 'string' ? new Date(date) : date;
            if (isNaN(dt.getTime()))
                return "—";
            var day = dt.getDate().toString().padStart(2, '0');
            var month = (dt.getMonth() + 1).toString().padStart(2, '0');
            var year = dt.getFullYear();
            return "".concat(day, ".").concat(month, ".").concat(year);
        }
        catch (_a) {
            return String(date);
        }
    };
    DateFormatter.toTime = function (date) {
        if (!date)
            return "—";
        try {
            var dt = typeof date === 'string' ? new Date(date) : date;
            if (isNaN(dt.getTime()))
                return "—";
            var hours = dt.getHours().toString().padStart(2, '0');
            var minutes = dt.getMinutes().toString().padStart(2, '0');
            return "".concat(hours, ":").concat(minutes);
        }
        catch (_a) {
            return "—";
        }
    };
    DateFormatter.formatDuration = function (minutes) {
        if (typeof minutes !== 'number' || minutes < 0) {
            return "—";
        }
        var hours = Math.floor(minutes / 60);
        var mins = minutes % 60;
        if (hours === 0) {
            return "".concat(mins, "\u043C");
        }
        return "".concat(hours, "\u0447 ").concat(mins, "\u043C");
    };
    DateFormatter.isValidDate = function (date) {
        return date instanceof Date && !isNaN(date.getTime());
    };
    return DateFormatter;
}());
export { DateFormatter };
//# sourceMappingURL=DateFormatter.js.map