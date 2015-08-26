=========================
akka clustering PoC notes
=========================

Sections
========

- `PoC abstract`_
- `Cluster Nodes`_
- `How to run`_
- `Observations`_
- `Deploying receiver on cloud platform`_

PoC abstract
============

I want to test the exchange of messages between distributed nodes where the actor being requested is not part of a shared namespace.  Why is this so important?
Let's take the following scenario:

    I have a product, currently being broken down into microservices.  I have a one web app that needs to talk to another.  This is straight forward enough when the actors are accessible via a share namespace.
    However, what if one of these actors, the receiver for instance, uses an esoteric implementation that cannot be shared across multiple applications because it is particular to that one web app?

This is what this tests; calling out to an actor, on 1 or more nodes in the cluster, that only exists on those nodes and nowhere else.


cluster Nodes
=============

- `Seed node`_ -> seed project
- `Worker nodes`_ -> node1, node2 projects

The `Shared`_ project contains the `AuditMessage` that is being passed between the `transmitter` and the `receiver` nodes.  This project also contains the `StartActor` and its supervisor, `StartSupervisorActor`.  I have included supervisor to manage the life cyle of the actors.

Seed node
---------

Working off of port 9001

**seed akka hocon:** ::

    akka {
       actor {
         provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"
       }

       remote {
         log-remote-lifecycle-events = DEBUG

         helios.tcp {
           transport-class = "Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote"
           applied-adapters = []
           transport-protocol = tcp
           #public-hostname = "POPULATE STATIC IP HERE"
           hostname = "127.0.0.1"
           port = 9001
         }
       }

       cluster {
         seed-nodes = ["akka.tcp://coach@127.0.0.1:9001"]
         roles = [seed]
         auto-down-unreachable-after = 10s
       }
     }

Worker nodes
------------

``node1`` is the transmitter node. ``node2`` x ? are the consumer nodes.  Both of these worker nodes connect to the seed node. ``node2`` has actor ``EventActor``.

**node1 akka hocon:** ::

    akka {
      actor {
        provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"

        deployment {
          /event {
                router = broadcast-group
                routees.paths = ["/user/e1"]
                nr-of-instances = 3
                cluster {
                    enabled = on
                    max-nr-of-instances-per-node = 20
                    allow-local-routees = on
                    use-role = receiver
                }
            }
        }
    }
    # here we're configuring the Akka.Remote module
    remote {
      helios.tcp {
          transport-class = "Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote"
          transport-protocol = tcp
          hostname = "127.0.0.1"
      }
    }
    cluster {
        seed-nodes = ["akka.tcp://coach@127.0.0.1:9001"]
        roles = [transmitter]
    }
  }


**node2 akka hocon:** ::

    akka {
         actor {
             provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"
         }
         remote {
             helios.tcp {
               port = 0
               hostname = "127.0.0.1"
            }
         }
         cluster {
             seed-nodes = ["akka.tcp://coach@127.0.0.1:9001"]
             roles = [receiver]
         }
     }


Shared
------

This library contains 1 Message - ``AuditMessage`` and 1 actor ``StartActor``.

How to run
==========

Start the ``seed`` and ``node1``.  then start up multiple ``node2`` s.


Observations
============

Errors are reported to the console rapidly when you key in ``M`` in the transmitter ``Node1`` console.  I was able to curtail these errors by applying the following change:

**node1 > Program.cs** ::

    private static int TransmitMessageManyTimes(int counter, ActorSelection start, int amount)
    {
       Console.Write($"Transmitting {amount:##,###} message(s) -> ");
       for (int i = 0; i < amount; i++)
       {
           counter++;
           start.Tell(new AuditMessage("Message no. - " + counter.ToString()));
       }
       Console.WriteLine(" [x] <- Transmitted message(s)");
       return counter;
    }

to ::

    private static int TransmitMessageManyTimes(int counter, ActorSelection start, int amount)
    {
       Console.Write($"Transmitting {amount:##,###} message(s) -> ");
       for (int i = 0; i < amount; i++)
       {
           counter++;
           start.Ask(new AuditMessage("Message no. - " + counter.ToString())).ContinueWith((m) =>
           {
           });
       }
       Console.WriteLine(" [x] <- Transmitted message(s)");
       return counter;
    }

My supposition here is that due to the weight of traffic, the actor never got the chance to respond to the seed node so it believed it was down and which then resulted in it being taken it out of circulation.  At least with the ``Ask`` implementation, the receiver actor has to respond to the sender actor and which means the seed acknowledges this communication and is satisfied that the receiver node is still active. I however cannot confirm this.


Deploying receiver on cloud platform
====================================

To run this on a VM on the Azure cloud platform please:

# apply the 3 IP addresses as prompted here in the curly brackets
# add port rule 8080 [arbitary port value] via Azure portal [or from CLI/PS] & add rule to machine firewall.

**node2 app.config** ::

  <akka>
    <hocon>
      <![CDATA[
          akka {
              actor {
                  provider = "Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"
              }
              remote {
                  helios.tcp {
		            transport-class = "Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote"
                    applied-adapters = []
                    transport-protocol = tcp
                    port = 8080
                    hostname = "{LOCAL_ADDRESS}"
                    public-hostname = "{PUBLIC_ADDRESS}"
                 }
              }
              cluster {
                  seed-nodes = ["akka.tcp://coach@{SEED_IP}:9001"]
                  roles = [receiver]
              }
          }
      ]]>
    </hocon>
  </akka>
