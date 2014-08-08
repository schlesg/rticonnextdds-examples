/*******************************************************************************
 (c) 2005-2014 Copyright, Real-Time Innovations, Inc.  All rights reserved.
 RTI grants Licensee a license to use, modify, compile, and create derivative
 works of the Software.  Licensee has the right to distribute object form only
 for use with RTI products.  The Software is provided "as is", with no warranty
 of any type, including any warranty for fitness for any purpose. RTI is under
 no obligation to maintain or support the Software.  RTI shall not be liable for
 any incidental or consequential damages arising out of the use or inability to
 use the software.
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
/* waitset_query_cond_subscriber.cs

   A subscription example

   This file is derived from code automatically generated by the rtiddsgen 
   command:

   rtiddsgen -language C# -example <arch> waitset_query_cond.idl

   Example subscription of type waitset_query_cond automatically generated by 
   'rtiddsgen'. To test them, follow these steps:

   (1) Compile this file and the example publication.

   (2) Start the subscription with the command
       objs\<arch>\waitset_query_cond_subscriber <domain_id> <sample_count>

   (3) Start the publication with the command
       objs\<arch>\waitset_query_cond_publisher <domain_id> <sample_count>

   (4) [Optional] Specify the list of discovery initial peers and 
       multicast receive addresses via an environment variable or a file 
       (in the current working directory) called NDDS_DISCOVERY_PEERS. 

   You can run any number of publishers and subscribers programs, and can 
   add and remove them dynamically from the domain.
                                   
   Example:
        
       To run the example application on domain <domain_id>:
                          
       bin\<Debug|Release>\waitset_query_cond_publisher <domain_id> 
                                                               <sample_count>  
       bin\<Debug|Release>\waitset_query_cond_subscriber <domain_id>
                                                               <sample_count>
              
       
modification history
------------ -------
*/

public class waitset_query_condSubscriber {
    public static void Main(string[] args) {

        // --- Get domain ID --- //
        int domain_id = 0;
        if (args.Length >= 1) {
            domain_id = Int32.Parse(args[0]);
        }

        // --- Get max loop count; 0 means infinite loop  --- //
        int sample_count = 0;
        if (args.Length >= 2) {
            sample_count = Int32.Parse(args[1]);
        }

        /* Uncomment this to turn on additional logging
        NDDS.ConfigLogger.get_instance().set_verbosity_by_category(
            NDDS.LogCategory.NDDS_CONFIG_LOG_CATEGORY_API, 
            NDDS.LogVerbosity.NDDS_CONFIG_LOG_VERBOSITY_STATUS_ALL);
        */

        // --- Run --- //
        try {
            waitset_query_condSubscriber.subscribe(
                domain_id, sample_count);
        }
        catch(DDS.Exception) {
            Console.WriteLine("error in subscriber");
        }
    }

    static void subscribe(int domain_id, int sample_count) {
        
        /* Auxiliary variables */
        String odd_string = "'ODD'";
        String even_string = "'EVEN'";

        // --- Create participant --- //

        /* To customize the participant QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.DomainParticipant participant =
            DDS.DomainParticipantFactory.get_instance().create_participant(
                domain_id,
                DDS.DomainParticipantFactory.PARTICIPANT_QOS_DEFAULT, 
                null /* listener */,
                DDS.StatusMask.STATUS_MASK_NONE);
        if (participant == null) {
            shutdown(participant);
            throw new ApplicationException("create_participant error");
        }

        // --- Create subscriber --- //

        /* To customize the subscriber QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Subscriber subscriber = participant.create_subscriber(
            DDS.DomainParticipant.SUBSCRIBER_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (subscriber == null) {
            shutdown(participant);
            throw new ApplicationException("create_subscriber error");
        }

        // --- Create topic --- //

        /* Register the type before creating the topic */
        System.String type_name = waitset_query_condTypeSupport.get_type_name();
        try {
            waitset_query_condTypeSupport.register_type(
                participant, type_name);
        }
        catch(DDS.Exception e) {
            Console.WriteLine("register_type error {0}", e);
            shutdown(participant);
            throw e;
        }

        /* To customize the topic QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Topic topic = participant.create_topic(
            "Example waitset_query_cond",
            type_name,
            DDS.DomainParticipant.TOPIC_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (topic == null) {
            shutdown(participant);
            throw new ApplicationException("create_topic error");
        }

        // --- Create reader --- //

        /* To customize the data reader QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.DataReader reader = subscriber.create_datareader(
            topic,
            DDS.Subscriber.DATAREADER_QOS_DEFAULT,
            null,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (reader == null) {
            shutdown(participant);
            throw new ApplicationException("create_datareader error");
        }
        waitset_query_condDataReader waitset_query_cond_reader =
            (waitset_query_condDataReader)reader;
        
        /* Create query condition */
        DDS.StringSeq query_parameters = new DDS.StringSeq(1);
        query_parameters.ensure_length(1, 1);

        /* The initial value of the parameters is EVEN string */
        query_parameters.set_at(0, even_string);

        String query_expression = "name MATCH %0";

        DDS.QueryCondition query_condition = 
            waitset_query_cond_reader.create_querycondition(
                DDS.SampleStateKind.NOT_READ_SAMPLE_STATE,
                DDS.ViewStateKind.ANY_VIEW_STATE,
                DDS.InstanceStateKind.ANY_INSTANCE_STATE,
                query_expression,
                query_parameters);
        if (query_condition == null) {
            shutdown(participant);
            throw new ApplicationException("create_querycondition error");
        }

        DDS.WaitSet waitset = new DDS.WaitSet();
        if (waitset == null) {
            shutdown(participant);
            throw new ApplicationException("create waitset error");
        }

        /* Attach Query Conditions */
        try {
            waitset.attach_condition((DDS.Condition)query_condition);
        } catch (DDS.Exception e) {
            Console.WriteLine("attach_condition error {0}", e);
            shutdown(participant);
            throw e;
        }

        DDS.Duration_t wait_timeout;
        wait_timeout.nanosec = (uint)500000000;
        wait_timeout.sec = 1;

        Console.WriteLine("\n>>>Timeout: {0} sec",
            wait_timeout.sec, wait_timeout.nanosec);
        Console.WriteLine(">>> Query conditions: name MATCH %0");
        Console.WriteLine("\t%0 = {0}", query_parameters.get_at(0));
        Console.WriteLine("---------------------------------\n");
    

        // --- Wait for data --- //

        /* Main loop */
        
        for (int count=0;
             (sample_count == 0) || (count < sample_count);
             ++count) {

            DDS.ConditionSeq active_conditions_seq = new DDS.ConditionSeq();

            /* We set a new parameter in the Query Condition after 7 secs */
            if (count == 7) {
                query_parameters.set_at(0,odd_string);
                Console.WriteLine("CHANGING THE QUERY CONDITION");
                Console.WriteLine("\n>>> Query conditions: name MATCH %0");
                Console.WriteLine("\t%0 = {0}", query_parameters.get_at(0));
                Console.WriteLine(">>> We keep one sample in the history");
                Console.WriteLine("-------------------------------------\n");
                query_condition.set_query_parameters(query_parameters);
            }

            /* wait() blocks execution of the thread until one or more attached
             * Conditions become true, or until a user-specified timeout 
             * expires.
             */
            try {
                waitset.wait(active_conditions_seq, wait_timeout);
            } catch (DDS.Retcode_Timeout) {
                Console.WriteLine("Wait timed out!! No conditions were " +
                    "triggered.");
                continue;
            } catch (DDS.Exception e) {
                Console.WriteLine("wait error {0}", e);
                break;
            }

            waitset_query_condSeq data_seq = new waitset_query_condSeq();
            DDS.SampleInfoSeq info_seq = new DDS.SampleInfoSeq();

            bool follow = true;
            while (follow) {
                try {
                    waitset_query_cond_reader.take_w_condition(
                        data_seq, info_seq,
	                    DDS.ResourceLimitsQosPolicy.LENGTH_UNLIMITED,
	                    query_condition);
	
                    for (int i = 0; i < data_seq.length; ++i) {
	                    if (!info_seq.get_at(i).valid_data) {
	                        Console.WriteLine("Got metadata");
	                        continue;
	                    }
	                    waitset_query_condTypeSupport.print_data(
	                        data_seq.get_at(i));
	                }
	            } catch (DDS.Retcode_NoData) {
	                /* When there isn't data, the subscriber stop to
                     * take samples
	                 */
	                follow = false;
	            } finally {
	                waitset_query_cond_reader.return_loan(data_seq, info_seq);
	            }
            }                    
        }

        // --- Shutdown --- //

        /* Delete all entities */
        shutdown(participant);
    }


    static void shutdown(
        DDS.DomainParticipant participant) {

        /* Delete all entities */

        if (participant != null) {
            participant.delete_contained_entities();
            DDS.DomainParticipantFactory.get_instance().delete_participant(
                ref participant);
        }

        /* RTI Connext provides finalize_instance() method on
           domain participant factory for users who want to release memory
           used by the participant factory. Uncomment the following block of
           code for clean destruction of the singleton. */
        /*
        try {
            DDS.DomainParticipantFactory.finalize_instance();
        }
        catch(DDS.Exception e) {
            Console.WriteLine("finalize_instance error {0}", e);
            throw e;
        }
        */
    }
}


