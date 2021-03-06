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
/* cft_publisher.cs

   A publication of data of type cft

   This file is derived from code automatically generated by the rtiddsgen 
   command:

   rtiddsgen -language C# -example <arch> cft.idl

   Example publication of type cft automatically generated by 
   'rtiddsgen'. To test them follow these steps:

   (1) Compile this file and the example subscription.

   (2) Start the subscription with the command
       objs\<arch>\cft_subscriber <domain_id> <sample_count>
                
   (3) Start the publication with the command
       objs\<arch>\cft_publisher <domain_id> <sample_count>

   (4) [Optional] Specify the list of discovery initial peers and 
       multicast receive addresses via an environment variable or a file 
       (in the current working directory) called NDDS_DISCOVERY_PEERS. 

   You can run any number of publishers and subscribers programs, and can 
   add and remove them dynamically from the domain.


   Example:

       To run the example application on domain <domain_id>:

       bin\<Debug|Release>\cft_publisher <domain_id> <sample_count>
       bin\<Debug|Release>\cft_subscriber <domain_id> <sample_count>

       
modification history
------------ -------       
*/

public class cftPublisher {

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
            cftPublisher.publish(
                domain_id, sample_count);
        }
        catch(DDS.Exception)
        {
            Console.WriteLine("error in publisher");
        }
    }

    static void publish(int domain_id, int sample_count) {

        // --- Create participant --- //

        /* To customize participant QoS, use 
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

        // --- Create publisher --- //

        /* To customize publisher QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Publisher publisher = participant.create_publisher(
        DDS.DomainParticipant.PUBLISHER_QOS_DEFAULT,
        null /* listener */,
        DDS.StatusMask.STATUS_MASK_NONE);
        if (publisher == null) {
            shutdown(participant);
            throw new ApplicationException("create_publisher error");
        }

        // --- Create topic --- //

        /* Register type before creating topic */
        System.String type_name = cftTypeSupport.get_type_name();
        try {
            cftTypeSupport.register_type(
                participant, type_name);
        }
        catch(DDS.Exception e) {
            Console.WriteLine("register_type error {0}", e);
            shutdown(participant);
            throw e;
        }

        /* To customize topic QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.Topic topic = participant.create_topic(
            "Example cft",
            type_name,
            DDS.DomainParticipant.TOPIC_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (topic == null) {
            shutdown(participant);
            throw new ApplicationException("create_topic error");
        }

        // --- Create writer --- //

        /* To customize data writer QoS, use 
           the configuration file USER_QOS_PROFILES.xml */
        DDS.DataWriter writer = publisher.create_datawriter(
            topic,
            DDS.Publisher.DATAWRITER_QOS_DEFAULT,
            null /* listener */,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (writer == null) {
            shutdown(participant);
            throw new ApplicationException("create_datawriter error");
        }

        /* If you want to set the reliability and history QoS settings
         * programmatically rather than using the XML, you will need to add
         * the following lines to your code and comment out the 
         * create_datawriter call above.
         */
        /*      
        DDS.DataWriterQos datawriter_qos = new DDS.DataWriterQos();
        try {
            publisher.get_default_datawriter_qos(datawriter_qos);
        } catch (DDS.Exception e) {
            Console.WriteLine("get_default_datawriter_qos error {0}", e);
            shutdown(participant);
            throw e;
        }
        
        datawriter_qos.reliability.kind = 
            DDS.ReliabilityQosPolicyKind.RELIABLE_RELIABILITY_QOS;
        datawriter_qos.durability.kind = 
            DDS.DurabilityQosPolicyKind.TRANSIENT_LOCAL_DURABILITY_QOS;
        datawriter_qos.history.kind = 
            DDS.HistoryQosPolicyKind.KEEP_LAST_HISTORY_QOS;
        datawriter_qos.history.depth = 20;

        DDS.DataWriter writer = publisher.create_datawriter(
            topic, datawriter_qos, null,
            DDS.StatusMask.STATUS_MASK_NONE);
        if (writer == null) {
            shutdown(participant);
            throw new ApplicationException("create_datawriter error");
        }
        */
        cftDataWriter cft_writer =
            (cftDataWriter)writer;

        // --- Write --- //

        /* Create data sample for writing */
        cft instance = cftTypeSupport.create_data();
        if (instance == null) {
            shutdown(participant);
            throw new ApplicationException(
                "cftTypeSupport.create_data error");
        }

        /* For a data type that has a key, if the same instance is going to be
           written multiple times, initialize the key here
           and register the keyed instance prior to writing */
        DDS.InstanceHandle_t instance_handle = DDS.InstanceHandle_t.HANDLE_NIL;
        /*
        instance_handle = cft_writer.register_instance(instance);
        */

        /* Main loop */
        const System.Int32 send_period = 1000; // milliseconds
        for (int count=0;
             (sample_count == 0) || (count < sample_count);
             ++count) {
            Console.WriteLine("Writing cft, count {0}", count);

            /* Modify the data to be sent here */
            if (count % 2 == 1) {
                instance.name = "ODD";
            } else {
                instance.name = "EVEN";
            }

            instance.count = count;

            try {
                cft_writer.write(instance, ref instance_handle);
            }
            catch(DDS.Exception e) {
                Console.WriteLine("write error {0}", e);
            }

            System.Threading.Thread.Sleep(send_period);
        }

        /*
        try {
            cft_writer.unregister_instance(
                instance, ref instance_handle);
        } catch(DDS.Exception e) {
            Console.WriteLine("unregister instance error: {0}", e);
        }
        */

        // --- Shutdown --- //

        /* Delete data sample */
        try {
            cftTypeSupport.delete_data(instance);
        } catch(DDS.Exception e) {
            Console.WriteLine(
                "cftTypeSupport.delete_data error: {0}", e);
        }

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
           domain participant factory for people who want to release memory
           used by the participant factory. Uncomment the following block of
           code for clean destruction of the singleton. */
        /*
        try {
            DDS.DomainParticipantFactory.finalize_instance();
        } catch (DDS.Exception e) {
            Console.WriteLine("finalize_instance error: {0}", e);
            throw e;
        }
        */
    }
}

