<?php


namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class UserpetsSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('user_pets')->insert([
            [
                'elder_user_id' => 1,
                'animals_id' => 1,
                'nickname' => 'ポチ',
            ],
        ]);
    }
}
