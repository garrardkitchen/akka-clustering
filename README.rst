=========================
akka clustering PoC notes
=========================

PoC abstract
============

I want to test the exchange of messages between distributed nodes where the actor being requests is not part of a shared namespace.  Why is this so important?
Let's take the following scenario.  I have a product, currently being broken down into microservices.  I have a one web app that needs to talk to another, .  This is straight forward enough if the actors are accessible via a share namespace.
What if one of these actors, the receiver for instance, uses an esoteric implementation that cannot be shared across applications because it is particular to that web app?  This is what this tests; calling out to an actor, on 1 or more nodes in the cluster, that only exists on those nodes.


Nodes
=====

    1 seed -> seed proj
    2 worker nodes -> node1, node2
    1 Shared -> message

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

2 worker nodes
--------------

``node1`` is the transmitter node. ``node2`` x ? are the consumer nodes.  Both of these worker nodes connect to the seed node.

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

To run
======

Start the ``seed`` and ``node1``.  then start up multiple ``node2``s.
