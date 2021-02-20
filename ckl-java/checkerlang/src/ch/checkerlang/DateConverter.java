/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
package ch.checkerlang;

import java.util.Calendar;
import java.util.Date;

public class DateConverter {
    public static final int[] DAYS_PER_MONTH = new int[]{31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
    public static final int DAYS_EPOCH = 25569;

    public static boolean isLeapYear(int year) {
        return (year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0));
    }

    public static int yearDays(int year) {
        return isLeapYear(year) ? 366 : 365;
    }

    public static int monthDays(int year, int month) {
        return isLeapYear(year) && month == 1 ? 29 : DAYS_PER_MONTH[month];
    }

    public static double  convertDateToOADate(Date date) {
        Calendar cal = Calendar.getInstance();
        cal.setTime(date);
        int year = cal.get(Calendar.YEAR);
        double result = 1;
        for (int y = 1900; y < year; y++) {
            result += isLeapYear(y) ? 366 : 365;
        }
        int month = cal.get(Calendar.MONTH);
        for (int m = 0; m < month; m++) {
            result += monthDays(year, m);
        }
        result += cal.get(Calendar.DAY_OF_MONTH);
        result += cal.get(Calendar.HOUR_OF_DAY) / 24.0;
        result += cal.get(Calendar.MINUTE) / 24.0 / 60.0;
        result += cal.get(Calendar.SECOND) / 24.0 / 60.0 / 60.0;
        result += cal.get(Calendar.MILLISECOND) / 24.0 / 60.0 / 60.0 / 1000.0;
        return result;
    }

    public static Date convertOADateToDate(double oadate) {
        double value = oadate - DAYS_EPOCH;
        int year = 1970;
        while (value > yearDays(year)) {
            value -= yearDays(year);
            year++;
        }
        int month = 0;
        while (value > monthDays(year, month)) {
            value -= monthDays(year, month);
            month++;
        }
        int day = (int) value + 1;
        value = value - (int) value;
        int hours = (int) (value * 24);
        value = value * 24 - hours;
        int minutes = (int) (value * 60);
        value = value * 60 - minutes;
        int seconds = (int) (value * 60);
        value = value * 60 - seconds;
        int milliseconds = (int) (value * 1000);
        Calendar cal = Calendar.getInstance();
        cal.set(year, month, day, hours, minutes, seconds);
        cal.set(Calendar.MILLISECOND, milliseconds);
        return cal.getTime();
    }

}
