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

/* waitset_statuscondSubscriber.java

   A publication of data of type waitset_statuscond

   This file is derived from code automatically generated by the rtiddsgen 
   command:

   rtiddsgen -language java -example <arch> .idl

   Example publication of type waitset_statuscond automatically generated by 
   'rtiddsgen' To test them follow these steps:

   (1) Compile this file and the example subscription.

   (2) Start the subscription on the same domain used for with the command
       java waitset_statuscondSubscriber <domain_id> <sample_count>

   (3) Start the publication with the command
       java waitset_statuscondPublisher <domain_id> <sample_count>

   (4) [Optional] Specify the list of discovery initial peers and 
       multicast receive addresses via an environment variable or a file 
       (in the current working directory) called NDDS_DISCOVERY_PEERS. 
       
   You can run any number of publishers and subscribers programs, and can 
   add and remove them dynamically from the domain.
              
                                   
   Example:
        
       To run the example application on domain <domain_id>:
            
       Ensure that $(NDDSHOME)/lib/<arch> is on the dynamic library path for
       Java.                       
       
        On UNIX systems: 
             add $(NDDSHOME)/lib/<arch> to the 'LD_LIBRARY_PATH' environment
             variable
                                         
        On Windows systems:
             add %NDDSHOME%\lib\<arch> to the 'Path' environment variable
                        

       Run the Java applications:
       
        java -Djava.ext.dirs=$NDDSHOME/class waitset_statuscondPublisher
                                                                 <domain_id>

        java -Djava.ext.dirs=$NDDSHOME/class waitset_statuscondSubscriber 
                                                                 <domain_id>  
       
       
modification history
------------ -------   
*/

import com.rti.dds.domain.*;
import com.rti.dds.infrastructure.*;
import com.rti.dds.subscription.*;
import com.rti.dds.topic.*;

// ===========================================================================

public class waitset_statuscondSubscriber {
    // -----------------------------------------------------------------------
    // Public Methods
    // -----------------------------------------------------------------------
    
    public static void main(String[] args) {
        // --- Get domain ID --- //
        int domainId = 0;
        if (args.length >= 1) {
            domainId = Integer.valueOf(args[0]).intValue();
        }
        
        // -- Get max loop count; 0 means infinite loop --- //
        int sampleCount = 0;
        if (args.length >= 2) {
            sampleCount = Integer.valueOf(args[1]).intValue();
        }
        
        
        /* Uncomment this to turn on additional logging
        Logger.get_instance().set_verbosity_by_category(
            LogCategory.NDDS_CONFIG_LOG_CATEGORY_API,
            LogVerbosity.NDDS_CONFIG_LOG_VERBOSITY_STATUS_ALL);
        */
        
        // --- Run --- //
        subscriberMain(domainId, sampleCount);
    }
    
    
    
    // -----------------------------------------------------------------------
    // Private Methods
    // -----------------------------------------------------------------------
    
    // --- Constructors: -----------------------------------------------------
    
    private waitset_statuscondSubscriber() {
        super();
    }
    
    
    // -----------------------------------------------------------------------
    
    private static void subscriberMain(int domainId, int sampleCount) {

        DomainParticipant participant = null;
        Subscriber subscriber = null;
        Topic topic = null;
        waitset_statuscondDataReader reader = null;

        try {

            // --- Create participant --- //
    
            /* To customize participant QoS, use
               the configuration file
               USER_QOS_PROFILES.xml */
    
            participant = DomainParticipantFactory.TheParticipantFactory.
                create_participant(
                    domainId, DomainParticipantFactory.PARTICIPANT_QOS_DEFAULT,
                    null /* listener */, StatusKind.STATUS_MASK_NONE);
            if (participant == null) {
                System.err.println("create_participant error\n");
                return;
            }                         

            // --- Create subscriber --- //
    
            /* To customize subscriber QoS, use
               the configuration file USER_QOS_PROFILES.xml */
    
            subscriber = participant.create_subscriber(
                DomainParticipant.SUBSCRIBER_QOS_DEFAULT, null /* listener */,
                StatusKind.STATUS_MASK_NONE);
            if (subscriber == null) {
                System.err.println("create_subscriber error\n");
                return;
            }     
                
            // --- Create topic --- //
        
            /* Register type before creating topic */
            String typeName = waitset_statuscondTypeSupport.get_type_name(); 
            waitset_statuscondTypeSupport.register_type(participant, typeName);
    
            /* To customize topic QoS, use
               the configuration file USER_QOS_PROFILES.xml */
    
            topic = participant.create_topic(
                "Example waitset_statuscond",
                typeName, DomainParticipant.TOPIC_QOS_DEFAULT,
                null /* listener */, StatusKind.STATUS_MASK_NONE);
            if (topic == null) {
                System.err.println("create_topic error\n");
                return;
            }                     
        
            // --- Create reader --- //

            /* To customize data reader QoS, use
               the configuration file USER_QOS_PROFILES.xml */
    
            reader = (waitset_statuscondDataReader)
                subscriber.create_datareader(
                    topic, Subscriber.DATAREADER_QOS_DEFAULT, null,
                    StatusKind.STATUS_MASK_NONE);
            if (reader == null) {
                System.err.println("create_datareader error\n");
                return;
            }                         
        
            /* Get status conditions
             * ---------------------
             * Each entity may have an attached Status Condition. To modify the
             * statuses we need to get the reader's Status Conditions first.
             */
            StatusCondition status_condition = reader.get_statuscondition();
            if (status_condition == null) {
                System.err.println("get_statuscondition error\n");
                return;
            }

            /* Set enabled statuses
             * --------------------
             * Now that we have the Status Condition, we are going to enable the
             * status we are interested in: knowing that data is available
             */
            status_condition.set_enabled_statuses(
                StatusKind.DATA_AVAILABLE_STATUS);

            /* Create and attach conditions to the WaitSet
             * -------------------------------------------
             * Finally, we create the WaitSet and attach both the Read 
             * Conditions and the Status Condition to it.
             */
            WaitSet waitset = new WaitSet();


            /* Attach Status Conditions */
            waitset.attach_condition(status_condition);

            // --- Wait for data --- //

            final long receivePeriodSec = 1;

            for (int count = 0; (sampleCount == 0) || (count < sampleCount);
                 ++count) {
                ConditionSeq active_conditions_seq = 
                    new ConditionSeq();
                Duration_t wait_timeout = new Duration_t();
                wait_timeout.sec = 1;
                wait_timeout.nanosec = 500000000;
            	
                try {
                    /* wait() blocks execution of the thread until one or more
                     * attached Conditions become true, or until a user- 
                     * specified timeout expires.
                     */
                    waitset.wait(active_conditions_seq, wait_timeout);
                    /* We get to timeout if no conditions were triggered */
                } catch (RETCODE_TIMEOUT e) {
                    System.out.println(
                          "Wait timed out!! No conditions were triggered.\n");
                    continue;
                }

                /* Get the number of active conditions */
                System.out.print("Got " + active_conditions_seq.size() + 
                    " active conditions\n");
                
                /* In this case, we have only a single condition, but
                   if we had multiple, we would need to iterate over
                   them and check which one is true.  Leaving the logic
                   for the more complex case. */
                for (int i = 0; i < active_conditions_seq.size(); ++i) {
                    

                    /* Compare with Status Condition (not required as we 
                     * only have one condition, but leaving the logic for the
                     * more complex case.) */
                    if (active_conditions_seq.get(i) == status_condition) {
                        /* Get the status changes so we can check which status
                         * condition triggered. */
                        int triggeredmask =
                                reader.get_status_changes();

                        /* Data available */
                        if ((triggeredmask & StatusKind.DATA_AVAILABLE_STATUS)
                                != 0) {
                            /* Current conditions match our conditions to read 
                             * data, so we can read data just like we would do  
                             * in any other example. */
                            waitset_statuscondSeq data_seq = 
                                new waitset_statuscondSeq();
                            SampleInfoSeq info_seq = new SampleInfoSeq();
    
                            /* Access data using read(), take(), etc.  If you 
                             * fail to do this the condition will remain true, 
                             * and the WaitSet will wake up immediately - 
                             * causing high CPU usage when it does not sleep in
                             * the loop */
                            boolean follow = true;
                            while (follow) {
                                try {
                                    reader.take(
                                       data_seq, info_seq, 
                                       ResourceLimitsQosPolicy.LENGTH_UNLIMITED,
                                       SampleStateKind.ANY_SAMPLE_STATE, 
                                       ViewStateKind.ANY_VIEW_STATE,
                                       InstanceStateKind.ANY_INSTANCE_STATE);
                
                                    /* Print data */
                                    for (int j = 0; j < data_seq.size(); ++j) {
                                        if (!((SampleInfo)
                                                info_seq.get(j)).valid_data) {
                                            System.out.println("Got metadata");
                                            continue;
                                        }
                                        System.out.println(
                                                ((waitset_statuscond)
                                                        data_seq.get(j))
                                                    .toString());
                                    }
                                } catch (RETCODE_NO_DATA noData) {
                                    /* When there isn't data, the subscriber
                                     * stop to take samples
                                     */
                                    follow = false;
                                } finally {
                                    /* Return the loaned data */
                                    reader.return_loan(data_seq, info_seq);
                                }
                            }
                        }
                    }
                }
                try {
                    Thread.sleep(receivePeriodSec * 1000);  // in millisec
                } catch (InterruptedException ix) {
                    System.err.println("INTERRUPTED");
                    break;
                }
            }
        } finally {

            // --- Shutdown --- //

            if(participant != null) {
                participant.delete_contained_entities();

                DomainParticipantFactory.TheParticipantFactory.
                    delete_participant(participant);
            }
            /* RTI Connext provides the finalize_instance()
               method for users who want to release memory used by the
               participant factory singleton. Uncomment the following block of
               code for clean destruction of the participant factory
               singleton. */
            //DomainParticipantFactory.finalize_instance();
        }
    }
    
    // -----------------------------------------------------------------------
    // Private Types
    // -----------------------------------------------------------------------
    
    // =======================================================================
    
}


        