================================
Test suite for ETS Trados Plugin
================================

-----------------
Running the tests
-----------------

In order for the tests to run correctly, the instance of ETS pointed to in the ApiUrl field of the string resource file
must be running ETS. The following fields will need to be updated with the appropriate criterea if you are running the
tests locally:
1) ApiHost
2) ApiKey
3) ApiUrl
4) Password
5) Username

To run the tests, either via the commandline run 
"""
C:/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE/MSTest.exe
/testcontainer:${PROJECT_SOURCE}/Plugins/TradosStudio/ETSTranslationProviderTests/bin/Debug/ETSTranslationProviderTests.dll
"""

or you can run it via visual studio (preferred as it's better to maneuver and diagnose issues):
Tests -> Run -> All Tests

To stop at breakpoints, choose:
Tests -> Debug -> All Tests

------------
Adding tests
------------

In order to add more tests, please add them according to what feature they are testing. The naming conventions of the 
test files are pretty self explanatory as they map a 1:1 relationship with the test files in ETSTranslationProvider.

The function names should each have the following format:

        [TestMethod]
        public void <ComponentBeingTested>_<StateBeingTested>_<ExpectedBehavior>()
        {
			// Testing components go here

            <Asserts>
        }

For instance, if we were testing a Sum function and wanted to test with two zeroes, our function could be
[TestMethod]
public void Sum_BothZeroes_EqualsZero()
{
	Assert.AreEqual(0, Sum(0, 0))
}

When you add a test, next time you run the tests, they will be automatically picked up and run.