package ch.checkerlang;

import org.junit.Assert;
import org.junit.Test;

import java.util.*;

public class TestDateHandling {
    @Test
    public void TestisLeapYear1() {
        Assert.assertFalse(DateConverter.isLeapYear(1999));
    }

    @Test
    public void TestisLeapYear2() {
        Assert.assertTrue(DateConverter.isLeapYear(1980));
    }

    @Test
    public void TestisLeapYear3() {
        Assert.assertFalse(DateConverter.isLeapYear(1900));
    }

    @Test
    public void TestisLeapYear4() {
        Assert.assertTrue(DateConverter.isLeapYear(2000));
    }

    @Test
    public void TestconvertDateToOADate1() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(1970, 0, 1)), 25569, 0);
    }

    @Test
    public void TestconvertDateToOADate2() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(2000, 5, 1)), 36678, 0);
    }

    @Test
    public void TestconvertDateToOADate3() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(2000, 5, 10)), 36687, 0);
    }

    @Test
    public void TestconvertDateToOADate4() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(1970, 0, 1, 12)), 25569.5, 0);
    }

    @Test
    public void TestconvertDateToOADate5() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(2000, 5, 1, 12)), 36678.5, 0);
    }

    @Test
    public void TestconvertDateToOADate6() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(2000, 5, 1, 12, 48, 36)), 36678.53375, 0.000001);
    }

    @Test
    public void TestconvertDateToOADate7() {
        Assert.assertEquals(DateConverter.convertDateToOADate(date(2000, 5, 1, 12, 48, 36, 444)), 36678.533755138895, 0.000000000000001);
    }

    @Test
    public void TestconvertOADateToDate1() {
        Assert.assertEquals(DateConverter.convertOADateToDate(25569).toString(), date(1970, 0, 1).toString());
    }

    @Test
    public void TestconvertOADateToDate2() {
        Assert.assertEquals(DateConverter.convertOADateToDate(36678).toString(), date(2000, 5, 1).toString());
    }

    @Test
    public void TestconvertOADateToDate3() {
        Assert.assertEquals(DateConverter.convertOADateToDate(36687).toString(), date(2000, 5, 10).toString());
    }

    @Test
    public void TestconvertOADateToDate4() {
        Assert.assertEquals(DateConverter.convertOADateToDate(25569.5).toString(), date(1970, 0, 1, 12).toString());
    }

    @Test
    public void TestconvertOADateToDate5() {
        Assert.assertEquals(DateConverter.convertOADateToDate(36678.5).toString(), date(2000, 5, 1, 12).toString());
    }

    @Test
    public void TestconvertOADateToDate6() {
        Assert.assertEquals(DateConverter.convertOADateToDate(36678.53375).toString(), date(2000, 5, 1, 12, 48, 36).toString());
    }

    @Test
    public void TestconvertOADateToDate7() {
        Assert.assertEquals(DateConverter.convertOADateToDate(36678.533755138895).toString(), date(2000, 5, 1, 12, 48, 36, 444).toString());
    }

    @Test
    public void TestroundTripOADate() {
        Assert.assertEquals(DateConverter.convertOADateToDate(DateConverter.convertDateToOADate(date(2017, 3, 5))).toString(), date(2017, 3, 5).toString());
    }

    @Test
    public void TestroundTripOADateMinus3() {
        Assert.assertEquals(DateConverter.convertOADateToDate(DateConverter.convertDateToOADate(date(2017, 3, 5)) - 3).toString(), date(2017, 3, 2).toString());
    }

    private Date date(int year, int month, int day) {
        Calendar cal = Calendar.getInstance();
        cal.set(year, month, day, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        return cal.getTime();
    }

    private Date date(int year, int month, int day, int hour) {
        Calendar cal = Calendar.getInstance();
        cal.set(year, month, day, hour, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        return cal.getTime();
    }

    private Date date(int year, int month, int day, int hour, int minute, int second) {
        Calendar cal = Calendar.getInstance();
        cal.set(year, month, day, hour, minute, second);
        cal.set(Calendar.MILLISECOND, 0);
        return cal.getTime();
    }

    private Date date(int year, int month, int day, int hour, int minute, int second, int milliseconds) {
        Calendar cal = Calendar.getInstance();
        cal.set(year, month, day, hour, minute, second);
        cal.set(Calendar.MILLISECOND, milliseconds);
        return cal.getTime();
    }
}
