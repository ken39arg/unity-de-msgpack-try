use Plack::Request;
use JSON::XS;
use Data::MessagePack;

my $serialize = {
    json => sub {
        JSON::XS::encode_json(shift);
    },
    msgpack => sub {
        Data::MessagePack->pack(shift);
    },
    default => sub {
        "";
    },
};

my $deserialize = {
    json => sub {
        JSON::XS::decode_json(shift);
    },
    msgpack => sub {
        Data::MessagePack->unpack(shift);
    },
    default => sub {
        {};
    },
};

my $app = sub {
    my $env = shift;

    my $req = Plack::Request->new($env);

    my $format = $req->param('format');
    $format = 'default' unless exists $serialize->{$format};

    my $data = {};

    if ($req->param('type') eq 'body') {
        $data = $deserialize->{$format}->($req->content);
    } elsif ($req->param('type') eq 'post') {
        $data = $req->body_parameters->as_hashref_mixed;
    }

    my $res = $req->new_response(200);
    $res->body( $serialize->{$format}->($data) );
    $res->finalize;
};
