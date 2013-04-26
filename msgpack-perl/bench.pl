#!/usr/bin/env perl
use strict;
use warnings;
use utf8;
use 5.010;
use Benchmark qw/:all/;
use JSON::XS;
use Data::MessagePack;
use Devel::Size qw/total_size/;
use HTTP::Request;
use LWP::UserAgent;

my $data = {};

my $count = 500;

for (1..$count) {
    $data->{"int$_"}    = $_;
    $data->{"long$_"}   = 2 ^ $_ + 2147483647;
    $data->{"float$_"}  = $_ / 10;
    $data->{"string$_"} = "test" . join("_", 1..$_);
    $data->{"bool$_"}   = $_ % 2 == 1 ? \1 : \0;
    $data->{"nil$_"}    = undef;
    $data->{"arr$_"}    = [1..$_];
    $data->{"map$_"}    = {
        int    => $_,
        long   => 2 ^ $_ + 2147483647,
        float  => $_ / 10,
        string => "test" . join("_", 1..$_),
        bool   => $_ % 2 == 1 ? \1 : \0,
        nil    => undef,
        arr    => ["a".."z"],
    };
}

my $json    = encode_json($data);
my $msgpack = Data::MessagePack->pack($data);

say "Size:";
say "   Hash   : " . total_size($data);
say "   JSON   : " . total_size($json);
say "   Msgpack: " . total_size($msgpack);

my $result = timethese(10000, {
    json    => sub {
        my $ua = LWP::UserAgent->new;
        my $req = HTTP::Request->new(POST => 'http://127.0.0.1:5511?type=body&format=json');
        $req->content($json);
        my $response = $ua->request($req);
    },
    msgpack => sub {
        my $ua = LWP::UserAgent->new;
        my $req = HTTP::Request->new(POST => 'http://127.0.0.1:5511?type=body&format=msgpack');
        $req->content($msgpack);
        my $response = $ua->request($req);
    },
    none    => sub {
        my $ua = LWP::UserAgent->new;
        my $req = HTTP::Request->new(POST => 'http://127.0.0.1:5511?type=body&format=default');
        $req->content("test");
        my $response = $ua->request($req);
    },
});

cmpthese $result;

__DATA__

@count=10

Size:
   Hash   : 35120
   JSON   : 3624
   Msgpack: 2600

      json: 54 wallclock secs (19.59 usr +  1.55 sys = 21.14 CPU) @ 473.04/s (n=10000)
   msgpack: 48 wallclock secs (17.63 usr +  1.31 sys = 18.94 CPU) @ 527.98/s (n=10000)
      none: 43 wallclock secs (16.79 usr +  1.20 sys = 17.99 CPU) @ 555.86/s (n=10000)

         Rate    json msgpack    none
json    473/s      --    -10%    -15%
msgpack 528/s     12%      --     -5%
none    556/s     18%      5%      --



@count=500

Size:
   Hash   : 6647886
   JSON   : 1552424
   Msgpack: 1474600

      json: 714 wallclock secs (146.21 usr + 44.58 sys = 190.79 CPU) @ 52.41/s (n=10000)
   msgpack: 782 wallclock secs (124.74 usr + 32.66 sys = 157.40 CPU) @ 63.53/s (n=10000)
      none: 46 wallclock secs (17.69 usr +  1.30 sys = 18.99 CPU) @ 526.59/s (n=10000)

          Rate    json msgpack    none
json    52.4/s      --    -18%    -90%
msgpack 63.5/s     21%      --    -88%
none     527/s    905%    729%      --
