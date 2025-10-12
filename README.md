# etvctl

This is an *experimental* app used to configure your [ErsatzTV](https://ersatztv.org) instance from the terminal.

## Setup

This app requires a `config.yml` file in the working directory with the following two fields:

```yaml
# the ersatztv server address
server: http://localhost:8409

# the template file used for exporting and planning
template: /tmp/smart_collections.yml
```

## Export

```shell
./etvctl export
```

The `export` command will export all smart collections from ErsatzTV into the configured template file.

## Plan

```shell
./etvctl plan
```

The `plan` command will compare the configured template file with the current state in ErsatzTV, and print the list of changes that need to be made to the ErsatzTV instance so that it matches the template.

## Apply

```shell
./etvctl apply
```

The `apply` command will generate and print a plan, and if approved by the user, apply the plan to the ErsatzTV instance.

