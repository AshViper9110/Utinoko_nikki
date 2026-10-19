<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class GamesSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('games')->insert([
            [
                'name' => 'ゲーム1',
            ],
            [
                'name' => 'ゲーム2',
            ],
            [
                'name' => 'ゲーム3',
            ],
        ]);
    }
}
