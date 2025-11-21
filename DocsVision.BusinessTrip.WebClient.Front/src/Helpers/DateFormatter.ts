export class DateFormatter {
    static toDisplayDateTime(dateTimeString: string | Date): string {
        if (!dateTimeString) return "—";

        try {
            const dt = typeof dateTimeString === 'string'
                ? new Date(dateTimeString)
                : dateTimeString;

            if (isNaN(dt.getTime())) return "—";

            const day = dt.getDate().toString().padStart(2, '0');
            const month = (dt.getMonth() + 1).toString().padStart(2, '0');
            const year = dt.getFullYear();
            const hours = dt.getHours().toString().padStart(2, '0');
            const minutes = dt.getMinutes().toString().padStart(2, '0');

            return `${day}.${month}.${year} ${hours}:${minutes}`;
        } catch {
            return String(dateTimeString);
        }
    }

    static toAPIFormat(date: Date): string {
        if (!(date instanceof Date) || isNaN(date.getTime())) {
            throw new Error("Некорректная дата");
        }

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    static formatDuration(minutes: number): string {
        if (typeof minutes !== 'number' || minutes < 0) {
            return "—";
        }

        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;

        if (hours === 0) {
            return `${mins}м`;
        }

        return `${hours}ч ${mins}м`;
    }

    static isValidDate(date: any): boolean {
        return date instanceof Date && !isNaN(date.getTime());
    }
}