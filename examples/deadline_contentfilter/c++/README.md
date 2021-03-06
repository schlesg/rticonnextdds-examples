# Example Code: Deadlines and Content Filtering

## Building C++ Example
Before compiling or running the example, make sure the environment variable
`NDDSHOME` is set to the directory where your version of *RTI Connext* is
installed.

Run *rtiddsgen* with the `-example` option and the target architecture of your
choice (e.g., *x64Win64VS2013* or *x64Linux3gcc4.8.2*). The *RTI Connext Core
Libraries and Utilities Getting Started Guide* describes this process in detail.
Follow the same procedure to generate the code and build the examples. **Do not
use the `-replace` option.** Assuming you want to generate an example for
*x64Win64VS2013* run:
```
rtiddsgen -language c++ -example x64Win64VS2013 deadline_contentfilter.idl
```

You will see messages that look like:
```
WARN com.rti.ndds.nddsgen.emitters.FileEmitter File exists and will not be
overwritten : /some/path/deadline_contentfilter_subscriber.cxx
WARN com.rti.ndds.nddsgen.emitters.FileEmitter File exists and will not be
overwritten : /some/path/deadline_contentfilter_publisher.cxx
```

This is normal and is only informing you that the subscriber/publisher code has
not been replaced, which is fine since all the source files for the example are
already provided.

Use the generated makefile of *Visual Studio* project to compile your
application.

## Running C++ Example
In two separate command prompt windows for the publisher and subscriber. Run
the following commands from the example directory (this is necessary to ensure
the application loads the QoS defined in *USER_QOS_PROFILES.xml*):

```sh
# Do not forget to replace the path separator to "\" on Windows.
objs/<arch_name>/deadline_contentfilter_publisher.exe  <domain_id> <samples_to_send>
objs/<arch_name>/deadline_contentfilter_subscriber.exe <domain_id> <sleep_periods>
```

The applications accept up to two arguments:

1. The `<domain_id>`. Both applications must use the same domain ID in order to
communicate. The default is 0.
2. How long the examples should run, measured in samples for the publisher
and sleep periods for the subscriber. A value of '0' instructs the
application to run forever; this is the default.

## Publisher Output
```
Writing instance0, x = 1, y = 1
Writing instance1, x = 1, y = 1
Writing instance0, x = 2, y = 2
Writing instance1, x = 2, y = 2
Writing instance0, x = 3, y = 3
Writing instance1, x = 3, y = 3
Writing instance0, x = 4, y = 4
Writing instance1, x = 4, y = 4
Writing instance0, x = 5, y = 5
Writing instance1, x = 5, y = 5
Writing instance0, x = 6, y = 6
Writing instance1, x = 6, y = 6
Writing instance0, x = 7, y = 7
Writing instance1, x = 7, y = 7
Writing instance0, x = 8, y = 8
Writing instance1, x = 8, y = 8
Writing instance0, x = 9, y = 9
Stopping writes to instance1
Offered deadline missed on instance code = 1
Writing instance0, x = 10, y = 10
Writing instance0, x = 11, y = 11
Offered deadline missed on instance code = 1
Writing instance0, x = 12, y = 12
Offered deadline missed on instance code = 1
Writing instance0, x = 13, y = 13
Writing instance0, x = 14, y = 14
Offered deadline missed on instance code = 1
Writing instance0, x = 15, y = 15
Offered deadline missed on instance code = 1
Writing instance0, x = 16, y = 16
```

## Subscriber Output
```
@ t=2.84s, Instance0: <1,1>
@ t=2.84s, Instance1: <1,1>
@ t=3.84s, Instance0: <2,2>
@ t=3.84s, Instance1: <2,2>
@ t=4.84s, Instance0: <3,3>
@ t=4.84s, Instance1: <3,3>
@ t=5.84s, Instance0: <4,4>
@ t=5.84s, Instance1: <4,4>
@ t=6.84s, Instance0: <5,5>
@ t=6.84s, Instance1: <5,5>
@ t=7.84s, Instance0: <6,6>
@ t=7.84s, Instance1: <6,6>
@ t=8.84s, Instance0: <7,7>
@ t=8.84s, Instance1: <7,7>
@ t=9.84s, Instance0: <8,8>
@ t=9.84s, Instance1: <8,8>
Starting to filter out instance1
@ t=10.84s, Instance0: <9,9>
@ t=11.84s, Instance0: <10,10>
Missed deadline @ t=11.86s on instance code = 1
@ t=12.84s, Instance0: <11,11>
@ t=13.84s, Instance0: <12,12>
Missed deadline @ t=13.86s on instance code = 1
@ t=14.84s, Instance0: <13,13>
@ t=15.84s, Instance0: <14,14>
Missed deadline @ t=15.86s on instance code = 1
@ t=16.84s, Instance0: <15,15>
@ t=17.84s, Instance0: <16,16>
Missed deadline @ t=17.86s on instance code = 1
Missed deadline @ t=19.86s on instance code = 1
Missed deadline @ t=21.88s on instance code = 1
```
