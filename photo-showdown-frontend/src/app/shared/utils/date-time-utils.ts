/**
 * DateTimeUtils
 */
export abstract class DateTimeUtils {
	/**
	 * Get the seconds until a date
	 * @param roundEndDate
	 * @returns
	 */
	static getSecondsUntil(roundEndDate: Date): number {
		const now = new Date();
		const diff = roundEndDate.getTime() - now.getTime();
		return Math.floor(diff / 1000);
	}

	/**
	 * Convert UTC to Local
	 * @param utcDate
	 * @returns
	 */
	static convertUtcToLocal(utcDate: Date): Date {
		return new Date(utcDate.toLocaleString());
	}

	/**
	 * Convert Local to UTC
	 * @param localDate
	 * @returns
	 */
	static convertLocalToUtc(localDate: Date): Date {
		return new Date(localDate.toUTCString());
	}
}
