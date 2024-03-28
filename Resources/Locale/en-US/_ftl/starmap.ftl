ship-ftl-jump-soon-message = Attention all crew. Ship coil spin initiated, course set to {$destination}. Prepare for jump in T-15 seconds.
ship-ftl-jump-jumped-message = Attention all crew. Ship coil spin stable. Re-entry into realspace ETA in T-60 seconds.
ship-ftl-jump-arrival-message = Attention all crew. Ship has officially re-entered realspace. Coolant systems initiated.

ship-destroyed-message = We are receiving no more inbound pings from {$ship}.
ship-inbound-message = Alert! Sensor array output have detected {$amount ->
      [one] a ship
      *[other] {$amount} ships
    } in the area. {$hostile ->
      [true] Hostilities confirmed!
      *[false] No hostile activity detected.
    } Automatically elevating to Blue alert.

ship-ftl-tag-star = STAR
ship-ftl-tag-base = BASE
ship-ftl-tag-planet = PLNT
ship-ftl-tag-ruin = RUIN
ship-ftl-tag-yard = YARD
ship-ftl-tag-gateway = GATE
ship-ftl-tag-warp = WARP
ship-ftl-tag-asteroid = ROID
ship-ftl-tag-oor = OUT OF RANGE

ship-state-tag-neutral = NTRL
ship-state-tag-hostile = HSTL

starmap-computer-title = Starmap Computer

starmap-details-display-label = Details
starmap-star-details-current-star = Current Star:
starmap-star-details-spin-range = Spin Range:
starmap-star-details-crystal-integrity = Integrity:

starmap-star-details-display-label = Selected Star
starmap-star-details-name = Name:
starmap-star-details-coordinates = Coordinates:

starmap-star-details-button-warp = Warp
starmap-star-details-position = {$x}-{$y}

starmap-center-station = Trading Outpost

starmap-alert-near = NEAR
starmap-alert-low = LOW
starmap-alert-danger = DANGER
starmap-alert-critical = ! CRITICAL !

drive-examined = The drive is {$charging ->
        *[false] not charging
        [true] charging
    } at {$charge}%. {$destination ->
        *[false] No
        [true] A
    } destination has been selected.

drive-examined-ready = The drive is ready for jump!

drive-examined-multiple-drives = [color=red]There are more than one drives on this ship, you must have only one![/color]
