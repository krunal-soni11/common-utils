namespace Common.Utils.Tests;

public class ValidateNullOrDefaultTests
{
    [Theory]
    // Value Types
    [InlineData(0, true)]
    [InlineData(5, false)]

    //[InlineData(false, true)]
    //[InlineData(true, false)]

    [InlineData('\0', true)]
    [InlineData('A', false)]

    // Nullable Value Types
    [InlineData(null, true)]
    public void Test_IsNullOrDefault_Int(int? value, bool expected)
    {
        Assert.Equal(expected, IsNullOrDefault(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Hello", false)]
    public void Test_IsNullOrDefault_String(string value, bool expected)
    {
        Assert.Equal(expected, IsNullOrDefault(value));
    }

    [Fact]
    public void Test_IsNullOrDefault_DateTime()
    {
        DateTime dt1 = default;
        DateTime dt2 = DateTime.Now;

        Assert.True(IsNullOrDefault(dt1));
        Assert.False(IsNullOrDefault(dt2));
    }

    [Fact]
    public void Test_IsNullOrDefault_NullableDateTime()
    {
        DateTime? dt1 = null;
        DateTime? dt2 = default(DateTime?);
        DateTime? dt3 = DateTime.Now;

        Assert.True(IsNullOrDefault(dt1));
        Assert.True(IsNullOrDefault(dt2));
        Assert.False(IsNullOrDefault(dt3));
    }

    [Fact]
    public void Test_IsNullOrDefault_ReferenceType()
    {
        MyClass obj1 = null;
        MyClass obj2 = new MyClass();

        Assert.True(IsNullOrDefault(obj1));
        Assert.False(IsNullOrDefault(obj2));
    }

    [Fact]
    public void Test_IsNullOrDefault_Collections()
    {
        List<int> list1 = null;
        List<int> list2 = new List<int>();
        List<int> list3 = new List<int> { 1, 2, 3 };

        Assert.True(IsNullOrDefault(list1)); // null
        Assert.False(IsNullOrDefault(list2)); // not default, just empty
        Assert.False(IsNullOrDefault(list3)); // not default
    }

    [Fact]
    public void Test_IsNullOrDefault_Struct()
    {
        MyStruct s1 = default;
        MyStruct s2 = new MyStruct { Value = 10 };

        Assert.True(IsNullOrDefault(s1));
        Assert.False(IsNullOrDefault(s2));
    }

    // Generic method reused
    private static bool IsNullOrDefault<T>(T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default(T));
    }

    public class MyClass
    {
        public int Value { get; set; }
    }

    public struct MyStruct
    {
        public int Value;
    }
}
