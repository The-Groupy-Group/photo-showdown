export class DateTimeUtils {
  // Convert UTC to Local
  static convertUtcToLocal(utcDate: Date): Date {
    // const offset = new Date().getTimezoneOffset();
    // return new Date(utcDate.getTime() - offset * 60000);
    return new Date(utcDate.toLocaleString());
  }

  // Convert Local to UTC
  static convertLocalToUtc(localDate: Date): Date {
    // const offset = new Date().getTimezoneOffset();
    // return new Date(localDate.getTime() + offset * 60000);
    return new Date(localDate.toUTCString());
  }
}
